using Microsoft.AspNetCore.Components;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Modal
{
    public partial class EditarRole : ComponentBase
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public string RoleId { get; set; } = string.Empty;
        [Inject] IRoleRepositorio RoleRepositorio { get; set; }

        private ApplicationRole rol = new();
        private List<Permisos> permisosDisponibles = new();
        private int nuevoPermisoId;

        protected override async Task OnInitializedAsync()
        {
            if (RoleRepositorio != null)
            {
                rol = await RoleRepositorio.ObtenerRolPorIdAsync(RoleId);
                if(rol != null)
                {
                    permisosDisponibles = await RoleRepositorio.ListaPermisos();

                    permisosDisponibles = permisosDisponibles
                        .Where(p => !rol.RolPermisos.Any(rp => rp.PermisosId == p.Id)).ToList();
                }               
            }
        }

        private async Task AgregarPermiso()
        {
            if (RoleRepositorio != null)
            {
                if (nuevoPermisoId == 0) return;

                var rolPermiso = new RolPermisos
                {
                    RoleId = rol.Id,
                    PermisosId = nuevoPermisoId
                };

                await RoleRepositorio.AsignarPermisosARol(new List<RolPermisos> { rolPermiso });
                rol = await RoleRepositorio.ObtenerRolPorIdAsync(RoleId);

                // Actualizar la lista de permisos disponibles
                permisosDisponibles = permisosDisponibles
                    .Where(p => p.Id != nuevoPermisoId)
                    .ToList();

                nuevoPermisoId = 0; // Reiniciar el selector
            }           
        }

        private async Task QuitarPermiso(int permisoId)
        {
            if(RoleRepositorio != null)
            {
                var rolPermiso = rol.RolPermisos.FirstOrDefault(rp => rp.PermisosId == permisoId);
                if (rolPermiso == null) return;

                await RoleRepositorio.QuitarPermisoDeRol(rolPermiso);
                rol.RolPermisos.Remove(rolPermiso);

                // Agregar el permiso de nuevo a la lista de permisos disponibles
                var permiso = await RoleRepositorio.ObtenerPermisoPorIdAsync(permisoId);
                if (permiso != null)
                {
                    permisosDisponibles.Add(permiso);
                }
            }          
        }
    }
}
