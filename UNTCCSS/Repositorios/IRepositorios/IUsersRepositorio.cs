using System.Security.Claims;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.ModelsDto;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface IUsersRepositorio
    {
        Task<Perfil> GetProfile(ApplicationUser user);
        Task<ApplicationUser> GetUserWithProfileAsync();
        Task<UsuariosDtoFilter> GetUsersAsync(int pagina);
        Task<UsuariosDtoFilter> GetUsersByEmpresaAsync(int pagina, int empresaId);
        Task<UsuariosDtoFilter> GetUsuariosFiltradosAsync(string filtroTipo, string filtroTexto, int pagina);
        Task<List<ApplicationRole>> ObtenerRoles();
        Task<bool> AddRoleUser(string RolId, string UserId);
        Task<bool> RemoveRoleUser(string RolId, string UserId);
        Task<bool> RemoveUser(string UserId);
        Task ReactivarUsuario(ApplicationUser user);
        Task<bool> AlternarEstado(string Id);
    }
}
