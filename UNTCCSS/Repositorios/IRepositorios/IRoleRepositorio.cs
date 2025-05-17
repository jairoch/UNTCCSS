using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.ModelsDto;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface IRoleRepositorio
    {
        Task<List<RolResumenDto>> GetResumenRolesAsync();
        Task<int> ObtenerTotalRolesAsync();
        Task<List<ApplicationRole>> ListaRolesPaginadosAsync(int pagina);
        Task<List<Permisos>> ListaPermisos();
        Task<ResponseSaveRole> CrearRol(ApplicationRole nuevoRol);
        Task AsignarPermisosARol(List<RolPermisos> rolPermisos);
        Task<ApplicationRole> ObtenerRolPorIdAsync(string roleId);
        Task<Permisos> ObtenerPermisoPorIdAsync(int permisoId);
        Task QuitarPermisoDeRol(RolPermisos rolPermiso);
        Task<bool> EliminarRolAsync(string roleId);
    }
}
