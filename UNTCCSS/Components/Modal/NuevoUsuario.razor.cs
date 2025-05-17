using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components.Authorization;
using UNTCCSS.Helper;
using UNTCCSS.Servicios.Email;
using UNTCCSS.ModelsDto;
using UNTCCSS.Components.Modal.ModalDto;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;
using UNTCCSS.Models;
using UNTCCSS.Repositorios;
using Blazored.Modal;

namespace UNTCCSS.Components.Modal
{
    public partial class NuevoUsuario
    {     
        [Inject] ILogger<NuevoUsuario> logger { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IUserStore<ApplicationUser> UserStore { get; set; }
        [Inject] IEmailSenderJCA EmailSender { get; set; }
        [Inject] IUsersRepositorio UsersService { get; set; }
        [Inject] IEmpresaRepositorio empresaService { get; set; }
        [Inject] IAutenticacionRepositorio AuthService {  get; set; }

        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] UserManager<ApplicationUser> UserManager { get; set; }
        [Inject] ApplicationDbContext db { get; set; }

        [Inject] AuthenticationStateProvider authenticationStateProvider { get; set; }

        private List<ComboBox> Empresas = new();
        
        [SupplyParameterFromForm]
        private NuevoUsuarioDto Input { get; set; } = new();

        private ReturnUser ReturnUser = new();
        
        [SupplyParameterFromQuery]
        private string ReturnUrl { get; set; }
        private bool IsProcessing = false;
        public string Message;
        private List<string> Roles = new();
        private List<string> Permisos = new();
        public int EmpresaId { get; set; }

        private AuthData authData = new();
        protected override async Task OnInitializedAsync()
        {
            authData = await AuthService.ObtenerDatosAutenticacionAsync();
            EmpresaId = authData.EmpresaId; 
            if (authData.Roles.Contains("Admin") || authData.Permisos.Contains("GestionarUsuarios"))
            {
                Empresas = await ObtenerEmpresasAsync();
            }
            else
            {
                await ObtenerEmpresaAsignadaAsync();
            }

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("SoloNumeros", "Input.Dni");
                await JSRuntime.InvokeVoidAsync("SoloNumeros", "Input.Telefono");
            }
        }
        public async Task RegisterUser(EditContext editContext)
        {
            if (db != null)
            {
                IsProcessing = true; 
                try
                {
                    var usuarioExistente = await ValidarEmail(Input.Email);
                    if (usuarioExistente != null)
                    {
                        //Si el usuario esta eliminado lo atualizara a false
                        //lo actualizara y luego retornara true en la propiedad
                        //para controlar aki en el if
                        if (usuarioExistente.IsDeleted)
                        {
                            ReturnUser.Success = true;
                            ReturnUser.Mensaje = "Success: Usuario registrado correctamente.";
                            ReturnUser.NuevoUsuario = Input;
                            EnviarEmail(usuarioExistente, usuarioExistente.Id);

                            await BlazoredModal.CloseAsync(ModalResult.Ok(ReturnUser));
                            return;
                        }
                        else
                        {
                            Message = "Lo sentimos, este correo ya se encuentra registrado.";
                            StateHasChanged();
                            return;
                        }
                    }

                    // Crear perfil y usuario nuevo
                    var perfil = CrearPerfil();
                    db.Add(perfil);
                    await db.SaveChangesAsync();

                    var user = CreateUser();
                    user.PerfilId = perfil.Id;
                    user.IdEmpresa = Input.IdEmpresa;
                    user.UserName = Guid.NewGuid().ToString("N").Substring(0, 10);
                    user.Email = Input.Email.Trim();
                    user.PhoneNumber = Input.Telefono.ToString();
                    user.IsDeleted = false;

                    var result = await UserManager.CreateAsync(user, Input.Password);
                    if (!result.Succeeded)
                    {
                        var errorMessages = string.Join(" | ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                        Message = $"Error al registrar usuario: {errorMessages}";
                        StateHasChanged();
                        return;
                    }

                    // Activamos la cuenta
                    user.LockoutEnabled = false;
                    await UserManager.UpdateAsync(user);

                    // Asignar rol por defecto
                    var defaultRole = "Predeterminado";
                    var role = await db.Roles.FirstOrDefaultAsync(r => r.NormalizedName == defaultRole.ToUpper());

                    if (role == null)
                    {
                        ReturnUser.Success = false;
                        ReturnUser.Mensaje = $"El rol '{defaultRole}' no existe.";

                        await BlazoredModal.CloseAsync(ModalResult.Ok(ReturnUser));
                        return;
                    }

                    var userRole = new ApplicationUserRoles
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };
                    db.UserRoles.Add(userRole);
                    await db.SaveChangesAsync();

                    var userId = await UserManager.GetUserIdAsync(user);
                    EnviarEmail(user, userId);

                    // Enviar respuesta al modal
                    ReturnUser.Success = true;
                    ReturnUser.UserId = userId;
                    ReturnUser.NuevoUsuario = Input;
                    ReturnUser.Mensaje = "Usuario registrado con éxito.";

                    await BlazoredModal.CloseAsync(ModalResult.Ok(ReturnUser));
                }
                finally
                {
                    IsProcessing = false; // Habilita el botón nuevamente
                }
            }
        }
        private async Task GenerarContraseñaSegura()
        {
            string password = await JSRuntime.InvokeAsync<string>("generateSecurePassword");
            Input.Password = password;
            Input.ConfirmPassword = password;
        }

        private async void EnviarEmail(ApplicationUser user, string userId)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("/account/confirmEmail").AbsoluteUri,
                new Dictionary<string, object> { ["UserId"] = userId, ["Token"] = encodedToken, ["returnUrl"] = ReturnUrl });

            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, callbackUrl, Input.Password);
        }
        private async Task<ApplicationUser> ValidarEmail(string email)
        {
            var existingUser = await UserManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                if (existingUser.IsDeleted)
                {
                    // Reactivar usuario
                    existingUser.IdEmpresa = Input.IdEmpresa;
                    existingUser.PhoneNumber = Input.Telefono.ToString();
                    existingUser.UserName = Input.Nombres;
                    existingUser.IsDeleted = false; // Activar cuenta
                    existingUser.LockoutEnabled = false; // Desbloquear cuenta

                    await UsersService.ReactivarUsuario(existingUser);
                    existingUser.IsDeleted = true; //Inportante para el if
                    return existingUser; // Usuario reactivado
                }
                else
                {
                    // Usuario ya registrado (no eliminado)
                    return existingUser;
                }
            }
            return null;
        }

        public Perfil CrearPerfil()
        {
            var perfil = new Perfil
            {
                Dni = Input.Dni,
                Nombres = TextFormater.ToTitleCase(Input.Nombres.Trim()),
                Apellidos = TextFormater.ToTitleCase(Input.Apellidos.Trim()),
                Telefono = Input.Telefono,
                Email = TextFormater.ToTitleCase(Input.Email.Trim()),
                Direccion = TextFormater.ToTitleCase(Input.Direccion.Trim()),
                ImagenPerfil = null
            };
            return perfil;
        }
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"No se puede crear una instancia de '{nameof(ApplicationUser)}'. " +
                    $"Asegúrate de que '{nameof(ApplicationUser)}' no sea una clase abstracta y tenga un constructor sin parámetros.");

            }
        }
        private async Task<List<ComboBox>> ObtenerEmpresasAsync()
        {
            return Empresas = await empresaService.ListaEmpresas();
        }
        private async Task<List<ComboBox>> ObtenerEmpresaAsignadaAsync()
        {
            return Empresas = await empresaService.ObtenerEmpresaPorId(EmpresaId);
        }
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!UserManager.SupportsUserEmail)
            {
                throw new NotSupportedException("La interfaz de usuario predeterminada requiere un almacén de usuarios con soporte para correo electrónico.");
            }
            return (IUserEmailStore<ApplicationUser>)UserStore;
        }
    }
}
