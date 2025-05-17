using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using Blazored.Modal.Services;
using Microsoft.VisualBasic;
using UNTCCSS.Components.Modal.ModalServicios;
using UNTCCSS.ModelsDto;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Components.Modal;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Pages.Admin
{
    public partial class Roles : ComponentBase
    {
        [Inject] IRoleRepositorio RoleRepositorio { get; set; }
        [Inject] private ConfirmacionServicio ConfirmacionService { get; set; }
        [CascadingParameter] public IModalService Modal { get; set; } = default!;
        private GuardarRol Input { get; set; } = new GuardarRol();
        List<ApplicationRole> ListaRoles = new();
        int TotalPaginas;
        int Pagina;

        public string Message;
        List<Permisos> ListPermisos = new();
        public Roles()
        {
            Pagina = 1;
        }
        private async Task EditarRol(string roleId)
        {
            var parameters = new ModalParameters();
            parameters.Add("Title", "Editar Roles");
            parameters.Add("RoleId", roleId);

            var options = new ModalOptions { Class = "modal-config" };
            var response = await Modal.Show<EditarRole>("Nuevo Usuario", parameters, options).Result;
        }

        protected override async Task OnInitializedAsync()
        {
            if(RoleRepositorio != null)
            {
                TotalPaginas = await RoleRepositorio.ObtenerTotalRolesAsync();
                ListPermisos = await RoleRepositorio.ListaPermisos();
                ListaRoles = await RoleRepositorio.ListaRolesPaginadosAsync(1);
            }
        }
        private void TogglePermiso(int permisoId, object value)
        { 
            bool isChecked = (bool)value; 
            if (isChecked)
            {
                
                if (!Input.PermisosSeleccionados.Contains(permisoId))
                    Input.PermisosSeleccionados.Add(permisoId);
            }
            else
            {
                Input.PermisosSeleccionados.Remove(permisoId);
            }
        }
        private async Task GuardarRol()
        {
            //Creamos un Rol
            if (RoleRepositorio == null) return;
            var nuevoRol = new ApplicationRole
            {
                Name = Input.Nombre.Trim(),
                NormalizedName = Input.Nombre.Trim().ToUpper(),
                Descripcion = Input.Descripcion.Trim(),
            };
            var response = await RoleRepositorio.CrearRol(nuevoRol);
            if (response.Role == null)
            {
                SetMessage(response.Message, 4);
                return;
            }

            //Asociamos permisos a un Rol
            var rolPermisos = Input.PermisosSeleccionados.Select(permisoId => new RolPermisos
            {
                RoleId = response.Role.Id,
                PermisosId = permisoId 
            }).ToList();

            await RoleRepositorio.AsignarPermisosARol(rolPermisos);
            SetMessage(response.Message, 4);
            ListaRoles = await RoleRepositorio.ListaRolesPaginadosAsync(Pagina);
        }
        private async Task CambiarPagina(int Pagina)
        {
            if(RoleRepositorio != null)
            {
                this.Pagina = Pagina;
                ListaRoles = await RoleRepositorio.ListaRolesPaginadosAsync(Pagina);
            }
        }
        private async void SetMessage(string mensaje, int segundos)
        {
            Message = mensaje;
            StateHasChanged();
            await Task.Delay(segundos * 1000);
            Message = null;
            StateHasChanged();
        }

        private async Task EliminarRol(string RolId)
        {
            string mensaje = "⚠️ Esta acción es irreversible. ¿Estás seguro de que deseas eliminar este rol?";
            mensaje += "\n\n🚨 *Importante:* Si hay usuarios asociados a este rol, serán afectados. ";
            mensaje += "Si un usuario no tiene otros roles asignados, se le asignará el *Rol Predeterminado* del sistema, ";
            mensaje += "el cual no tiene permisos para realizar operaciones. Proceda con precaución.";

            bool aceptado = await ConfirmacionService.MostrarConfirmacion("Eliminar Rol", mensaje, "error");

            if (aceptado)
            {
                bool response = await RoleRepositorio.EliminarRolAsync(RolId);
                if (response)
                {
                    var rolEliminado = ListaRoles.FirstOrDefault(r => r.Id == RolId);
                    if (rolEliminado != null)
                    {
                        ListaRoles.Remove(rolEliminado);
                    }
                    string mensajeExito = "✅ El rol ha sido eliminado correctamente.";
                    SetMessage(mensajeExito, 4);
                }
                else
                {
                    string mensajeError = "⚠️ No se pudo eliminar el rol. Puede que no exista o tenga dependencias.";
                    SetMessage(mensajeError, 4);
                }
            }
        }

        private async Task TogglePermiso(int PermisoId, bool Valor, int permisoId)
        {
            // Simular la actualización del permiso
            await Task.Run(() =>
            {
                Console.WriteLine($"Permiso ID: {PermisoId}, Acción: {Valor}");
                // Aquí puedes agregar la lógica para actualizar el permiso en la base de datos
            });
        }
    }
}
