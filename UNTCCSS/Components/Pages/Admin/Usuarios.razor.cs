using Blazored.Modal.Services;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using UNTCCSS.Components.Modal;
using UNTCCSS.ModelsDto;
using UNTCCSS.Components.Modal.ModalDto;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;
using UNTCCSS.Repositorios;

namespace UNTCCSS.Components.Pages.Admin
{
    public partial class Usuarios : ComponentBase
    {
        [CascadingParameter] public IModalService Modal { get; set; } = default!;
        private string Message;
        private async void MostrarModal()
        {
            var parameters = new ModalParameters();
            parameters.Add("Title", "Nuevo Usuario");

            var options = new ModalOptions { Class = "modal-config" };

            var modalResult = await Modal.Show<NuevoUsuario>("Nuevo Usuario", parameters, options).Result;

            if (!modalResult.Cancelled && modalResult.Data is ReturnUser returnUser)
            {
                if (returnUser.Success)
                {
                    if (returnUser.NuevoUsuario != null)
                    {
                        var nuevoUsuario = new UsuariosDto
                        {
                            Id = returnUser.UserId,
                            Dni = returnUser.NuevoUsuario.Dni,
                            Nombres = returnUser.NuevoUsuario.Nombres,
                            Apellidos = returnUser.NuevoUsuario.Apellidos,
                            Correo = returnUser.NuevoUsuario.Email,
                            Blokeado = false,
                            Roles = new List<ApplicationRole>
                            {
                                new ApplicationRole{Name = "Predeterminado"}
                            },
                            Permisos = new List<Permisos>()
                        };
                        ListaUsuarios.Add(nuevoUsuario);
                        StateHasChanged();
                    }
                    SetMessage(returnUser.Mensaje, 4);
                }
                else
                {
                    SetMessage(returnUser.Mensaje, 4);
                }
            }
        }
        [Inject] IUsersRepositorio UserService { get; set; }
        [Inject] IRoleRepositorio RoleService { get; set; }
        [Inject] ILogger<Usuarios> logger { get; set; }
        [Inject] IAutenticacionRepositorio AuthService { get; set; }

        private UsuariosDtoFilter Tabla = new();
        private List<UsuariosDto> ListaUsuarios = new();
        private List<RolResumenDto> RolesResumen = new();

        private string UserId;
        public int EmpresaId { get; set; }

        private string filtroTexto = string.Empty;
        private string filtroTipo = "Dni";
        private int Pagina = 1;
        private int TotalPaginas;
        private async void SetMessage(string mensaje, int segundos)
        {
            Message = mensaje;
            StateHasChanged();
            await Task.Delay(segundos * 1000);
            Message = null;
            StateHasChanged();
        }
        private AuthData authData = new();
        protected override async Task OnInitializedAsync()
        {
            authData = await AuthService.ObtenerDatosAutenticacionAsync();
            UserId = authData.UserId;
            EmpresaId = authData.EmpresaId;

            if (authData.Roles.Contains("Admin") || authData.Permisos.Contains("VerUsuariosAdmin"))
            {
                await CargarUsuarios(1);
            }
            else
            {
                await CargarUsuariosEmpresaAsignada(1);
            }
        }
        private async Task CargarUsuarios(int nuevaPagina)
        {
            if (UserService != null)
            {
                Pagina = nuevaPagina;
                Tabla = await UserService.GetUsersAsync(Pagina);
                ListaUsuarios = Tabla.Usuarios;
                TotalPaginas = Tabla.TotalPaginas;

            }
            if(RoleService != null)
            {
                RolesResumen = await RoleService.GetResumenRolesAsync();
            }
        }
        private async Task CargarUsuariosEmpresaAsignada(int NewPage)
        {
            if(UserService != null)
            {
                var user = 

                Pagina = NewPage;
                Tabla = await UserService.GetUsersByEmpresaAsync(Pagina, EmpresaId);
                ListaUsuarios = Tabla.Usuarios;
                TotalPaginas = Tabla.TotalPaginas;
            }
        }
        private async Task BuscarUsuarios()
        {
            if (UserService != null)
            {
                Pagina = 1;
                var resultado = await UserService.GetUsuariosFiltradosAsync(filtroTipo, filtroTexto, Pagina);
                ListaUsuarios = resultado.Usuarios;
                TotalPaginas = resultado.TotalPaginas;
            }
        }
        private async Task RestablecerFiltro()
        {
            filtroTexto = string.Empty;
            filtroTipo = "Dni";
            await CargarUsuarios(1);
        }
        private async Task CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina >= 1 && nuevaPagina <= TotalPaginas)
            {
                await CargarUsuarios(nuevaPagina);
            }
        }
        private async Task ConfigurationRole(string Id)
        {
            var user = ListaUsuarios.FirstOrDefault(u => u.Id == Id);
            if(user != null)
            {
                var parameters = new ModalParameters();
                parameters.Add("User", user);

                var options = new ModalOptions { Class = "modal-config" };

                var modalResult = await Modal.Show<ConfiguracionRoles>("Configuracion de Usuarios y Roles", parameters, options).Result;
            }
            else
            {
                Message = "El usuario no fue encontrado en la lista.";
            }
        }

        private async Task CambiarEstado(string Id)
        {
            if (UserService != null)
            {
                bool response = await UserService.AlternarEstado(Id);
                if (response)
                {
                    var usuario = ListaUsuarios.FirstOrDefault(u => u.Id == Id);
                    if (usuario != null)
                    {
                        usuario.Blokeado = !usuario.Blokeado; 
                        StateHasChanged();
                    }
                    SetMessage("Estado del usuario actualizado", 4);
                }
                else
                {
                    SetMessage("Error al actualizar el estado del usuario.", 4);
                }
            }
        }
        private async Task DeleteUser(string Id)
        {
            if(UserService != null)
            {
                bool response = await UserService.RemoveUser(Id);
                if(response)
                {
                    var usuario = ListaUsuarios.FirstOrDefault(u => u.Id == Id);
                    if (usuario != null)
                    {
                        ListaUsuarios.Remove(usuario);
                        StateHasChanged();
                    }
                    SetMessage("Usuario eliminado correctamente.", 4);
                }
                else
                {
                    SetMessage("Error al eliminar el usuario.", 4);
                }
            }
        }
    }
}
