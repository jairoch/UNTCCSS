using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UNTCCSS.Data;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface IAutenticacionRepositorio
    {
        Task<AuthData> ObtenerDatosAutenticacionAsync();
        Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user);
        Task<IdentityResult> ConfirmarEmailAsync(ApplicationUser user, string decodedToken);
        string GenerarTokenConsultaCert(int estudianteId);
        int DecodificarTokenConsultaCert(string token);
    }
}
