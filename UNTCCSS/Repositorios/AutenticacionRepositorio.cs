using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{
    public class AutenticacionRepositorio : IAutenticacionRepositorio
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDbContextFactory<ApplicationDbContext> db;
        private readonly IConfiguration configuration;
        string key;
        public AutenticacionRepositorio(UserManager<ApplicationUser> userManager,
            IDbContextFactory<ApplicationDbContext> db,
            IConfiguration configuration,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.userManager = userManager;
            this.configuration = configuration;
            this.db = db;

            key = configuration.GetValue<string>("SecretKeyJwt:Key");
        }
        public async Task<AuthData> ObtenerDatosAutenticacionAsync()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
                var permisos = user.FindAll(c => c.Type == "permission").Select(c => c.Value).ToList();
                var empresaId = int.TryParse(user.FindFirst(c => c.Type == "empresa")?.Value, out var eid) ? eid : 0;

                return new AuthData
                {
                    UserId = userId ?? string.Empty,
                    Roles = roles,
                    Permisos = permisos,
                    EmpresaId = empresaId
                };
            }
            return new AuthData();
        }
        //Metodo que se usa en el login
        public async Task<List<Claim>> GetUserClaimsAsync(ApplicationUser user)
        {
            using var context = db.CreateDbContext();
            var claims = new List<Claim>();
            var roles = await userManager.GetRolesAsync(user);

            var permisos = await context.RolPermisos
                .Where(rp => roles.Contains(rp.Role.Name))
                .Include(rp => rp.Permisos)
                .Select(rp => rp.Permisos.Name)
                .Distinct()
                .ToListAsync();

            // Agregar Claims Personalizados            
            foreach (var permiso in permisos)
            {
                claims.Add(new Claim("permission", permiso));
            }
            claims.Add(new Claim("empresa", user.IdEmpresa.ToString()));
            return claims;
        }
        public async Task<IdentityResult> ConfirmarEmailAsync(ApplicationUser user, string decodedToken)
        {
            var esValido = await ValidarTokenEmailAsync(user, decodedToken);
            if (!esValido)
            {
                return IdentityResult.Failed(new IdentityError { Description = "❌ Token de confirmación incorrecto o expirado." });
            }

            using var context = db.CreateDbContext();
            user.EmailConfirmed = true;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return IdentityResult.Success;
        }
        public async Task<bool> ValidarTokenEmailAsync(ApplicationUser user, string decodedToken)
        {
            return await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation", decodedToken);
        }
        public string GenerarTokenConsultaCert(int estudianteId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("EstudianteId", estudianteId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(30), // Expira en 30 min
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public int DecodificarTokenConsultaCert(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            try
            {
                var principal = handler.ValidateToken(token, validations, out var validatedToken);
                var idClaim = principal.Claims.FirstOrDefault(c => c.Type == "EstudianteId");
                return idClaim != null ? int.Parse(idClaim.Value) : 0;
            }
            catch
            {
                return 0; // Token inválido
            }
        }

    }
    public class AuthData
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permisos { get; set; } = new();
        public int EmpresaId { get; set; }
    }
}

