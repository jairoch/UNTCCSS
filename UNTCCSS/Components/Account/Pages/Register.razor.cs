using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using UNTCCSS.Data;
using UNTCCSS.Models;

namespace UNTCCSS.Components.Account.Pages
{
    public partial class Register
    {
        [Inject] ILogger<Register> logger { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ApplicationDbContext dbContext { get; set; }

        private IEnumerable<IdentityError>? identityErrors;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        private string? Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";

        public async Task RegisterUser(EditContext editContext)
        {
            var perfil = CrearPerfil();
            dbContext.Add(perfil);
            dbContext.SaveChanges();

            var user = CreateUser();
            user.PerfilId = perfil.Id;

            await UserStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            user.NormalizedUserName = perfil.Nombres.ToUpper().Trim();
            var result = await UserManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                identityErrors = result.Errors;
                return;
            }

            logger.LogInformation("El usuario creó una nueva cuenta con contraseña.");

            // Obtener el RoleId del rol "USUARIO"
            var defaultRole = "USUARIO";
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.NormalizedName == defaultRole.ToUpper());

            if (role == null)
            {
                identityErrors = new List<IdentityError>
                {
                    new IdentityError { Description = $"El rol '{defaultRole}' no existe." }
                };
                return;
            }

            // Asignar el rol al usuario
            var userRole = new ApplicationUserRoles
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            dbContext.UserRoles.Add(userRole);
            await dbContext.SaveChangesAsync();

            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = ReturnUrl });

            await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            if (UserManager.Options.SignIn.RequireConfirmedAccount)
            {
                RedirectManager.RedirectTo(
                    "Account/RegisterConfirmation",
                    new() { ["email"] = Input.Email, ["returnUrl"] = ReturnUrl });
            }

            await SignInManager.SignInAsync(user, isPersistent: false);
            RedirectManager.RedirectTo(ReturnUrl);
        }
        public Perfil CrearPerfil()
        {
            var perfil = new Perfil
            {
                Dni = Input.Dni,
                Nombres = Input.Nombres,
                Apellidos = Input.Apellidos,
                Telefono = Input.Telefono,
                Email = Input.Email,
                Direccion = Input.Direccion,
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

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!UserManager.SupportsUserEmail)
            {
                throw new NotSupportedException("La interfaz de usuario predeterminada requiere un almacén de usuarios con soporte para correo electrónico.");
            }
            return (IUserEmailStore<ApplicationUser>)UserStore;
        }


        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = "";

            [Required(ErrorMessage = "El DNI es Requerido")]
            public string Dni {  get; set; }

            [Required(ErrorMessage = "Nombres son obligatorios")]
            public string Nombres { get; set; } = "";

            [Required(ErrorMessage = "Apellidos son obligatorios")]
            public string Apellidos { get; set; } = "";

            [Required(ErrorMessage = "Telefono obligatorio")]
            public int Telefono { get; set; }

            [Required(ErrorMessage = "Direccion requerida")]
            public string Direccion { get; set; } = "";
        }
    }
}
