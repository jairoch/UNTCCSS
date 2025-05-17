using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.ModelsDto;
using UNTCCSS.Repositorios.IRepositorios;
namespace UNTCCSS.Repositorios
{
    public class UsersRepositorio : IUsersRepositorio
    {
        AuthenticationStateProvider AuthenticationStateProvider;
        private readonly IDbContextFactory<ApplicationDbContext> db;
        private readonly UserManager<ApplicationUser> userManager;
        private static readonly SemaphoreSlim Lock = new(1, 1);
        public UsersRepositorio(IDbContextFactory<ApplicationDbContext> db, UserManager<ApplicationUser> userManager, AuthenticationStateProvider AuthenticationStateProvider)
        {
            this.db = db;
            this.userManager = userManager;
            this.AuthenticationStateProvider = AuthenticationStateProvider;           
        }
        private async Task<AuthenticationState> ObtenerEstadodeAutenticacion()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            return authState;
        }
        public async Task<ApplicationUser> GetUserWithProfileAsync()
        {
            await Lock.WaitAsync();
            try
            {
                var userClaims = await ObtenerEstadodeAutenticacion();
                var user = await userManager.GetUserAsync(userClaims.User);
                if (user != null)
                {
                    using var context = db.CreateDbContext();
                    return await context.Users
                        .Include(u => u.Perfil)
                        .FirstOrDefaultAsync(u => u.Id == user.Id);
                }
                return null;
            } 
            finally 
            { 
                Lock.Release(); 
            }
        }
        public async Task<UsuariosDtoFilter> GetUsersAsync(int pagina)
        {
            using var context = db.CreateDbContext();
            int registrosPorPagina = 15;
            int skip = (pagina - 1) * registrosPorPagina;

            // Obtener el total de registros antes de aplicar la paginación
            int totalRegistros = await context.Users.Where(i => i.IsDeleted == false).CountAsync();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            // Consulta optimizada con paginación y orden descendente
            var users = await context.Users
                .Where(i => i.IsDeleted == false)
                .OrderByDescending(u => u.Id)
                .Skip(skip)
                .Take(registrosPorPagina)
                .Include(u => u.Perfil)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permisos)
                .ToListAsync();

            // Mapear a DTO
            var userDtos = users.Select(user => new UsuariosDto
            {
                Id = user.Id,
                Dni = user.Perfil.Dni,
                Nombres = user.Perfil.Nombres,
                Apellidos = user.Perfil.Apellidos,
                Correo = user.Email,
                Blokeado = user.LockoutEnabled,
                Roles = user.UserRoles.Select(ur => ur.Role).ToList(),
                Permisos = user.UserRoles
                    .SelectMany(ur => ur.Role.RolPermisos)
                    .Select(rp => rp.Permisos)
                    .Distinct()
                    .ToList()
            }).ToList();

            return new UsuariosDtoFilter
            {
                TotalPaginas = totalPaginas,
                Usuarios = userDtos
            };
        }
        public async Task<UsuariosDtoFilter> GetUsersByEmpresaAsync(int pagina, int empresaId)
        {
            using var context = db.CreateDbContext();
            int registrosPorPagina = 15;
            int skip = (pagina - 1) * registrosPorPagina;

            // Obtener el total de registros filtrados por empresa
            int totalRegistros = await context.Users
                .Where(u => u.IdEmpresa == empresaId && u.IsDeleted == false)
                .CountAsync();

            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            // Consulta optimizada con paginación y filtrado por empresa
            var users = await context.Users
                .Where(u => u.IdEmpresa == empresaId && u.IsDeleted == false)
                .OrderByDescending(u => u.Id)
                .Skip(skip)
                .Take(registrosPorPagina)
                .Include(u => u.Perfil)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permisos)
                .ToListAsync();

            // Mapear a DTO
            var userDtos = users.Select(user => new UsuariosDto
            {
                Id = user.Id,
                Dni = user.Perfil.Dni,
                Nombres = user.Perfil.Nombres,
                Apellidos = user.Perfil.Apellidos,
                Correo = user.Email,
                Blokeado = user.LockoutEnabled,
                Roles = user.UserRoles.Select(ur => ur.Role).ToList(),
                Permisos = user.UserRoles
                    .SelectMany(ur => ur.Role.RolPermisos)
                    .Select(rp => rp.Permisos)
                    .Distinct()
                    .ToList()
            }).ToList();

            return new UsuariosDtoFilter
            {
                TotalPaginas = totalPaginas,
                Usuarios = userDtos
            };
        }
        public async Task<UsuariosDtoFilter> GetUsuariosFiltradosAsync(string filtroTipo, string filtroTexto, int pagina)
        {
            using var context = db.CreateDbContext();

            var query = context.Users
                .Include(u => u.Perfil)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permisos)
                .AsQueryable();

            // Aplicar filtros según el tipo seleccionado
            if (!string.IsNullOrWhiteSpace(filtroTexto))
            {
                switch (filtroTipo)
                {
                    case "Dni":
                        query = query.Where(u => u.Perfil.Dni.Contains(filtroTexto));
                        break;
                    case "Nombre":
                        query = query.Where(u => u.Perfil.Nombres.Contains(filtroTexto));
                        break;
                    case "Roles":
                        query = query.Where(u => u.UserRoles.Any(ur => ur.Role.Name.Contains(filtroTexto)));
                        break;
                }
            }

            // Obtener el total de registros después de aplicar los filtros
            int totalRegistros = await query.CountAsync();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / 15);

            // Aplicar paginación y orden descendente
            var usuarios = await query
                .OrderByDescending(u => u.Id)
                .Skip((pagina - 1) * 15)
                .Take(15)
                .Select(user => new UsuariosDto
                {
                    Id = user.Id,
                    Dni = user.Perfil.Dni,
                    Nombres = user.Perfil.Nombres,
                    Apellidos = user.Perfil.Apellidos,
                    Correo = user.Email,
                    Blokeado = user.LockoutEnabled,
                    Roles = user.UserRoles.Select(ur => ur.Role).ToList(),
                    Permisos = user.UserRoles
                        .SelectMany(ur => ur.Role.RolPermisos)
                        .Select(rp => rp.Permisos)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            return new UsuariosDtoFilter
            {
                TotalPaginas = totalPaginas,
                Usuarios = usuarios
            };
        }
        public async Task<Perfil> GetProfile(ApplicationUser user)
        {
            using var context = db.CreateDbContext();
            return await context.Perfil
                .FirstOrDefaultAsync(p => p.Id == user.PerfilId);
        }
        public async Task<List<ApplicationRole>> ObtenerRoles()
        {
            using var context = db.CreateDbContext();
            return await context.Roles.ToListAsync();
        }
        public async Task<bool> AddRoleUser(string RolId, string UserId)
        {
            using var context = db.CreateDbContext();

            try
            {
                var userRole = new ApplicationUserRoles
                {
                    UserId = UserId,
                    RoleId = RolId
                };

                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();

                // Actualiza el security stamp
                var user = await userManager.FindByIdAsync(UserId);
                if (user != null)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public async Task<bool> RemoveRoleUser(string RolId, string UserId)
        {
            using var context = db.CreateDbContext();

            try
            {
                var userRole = await context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == UserId && ur.RoleId == RolId);

                if (userRole != null)
                {
                    context.UserRoles.Remove(userRole);
                    await context.SaveChangesAsync();

                    // Actualiza el security stamp
                    var user = await userManager.FindByIdAsync(UserId);
                    if (user != null)
                    {
                        await userManager.UpdateSecurityStampAsync(user);
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public async Task<bool> RemoveUser(string UserId)
        {
            using var context = db.CreateDbContext();
            var user = await context.Users.FindAsync(UserId);

            if (user == null)
                return false;

            // Eliminar los roles asociados al usuario
            var userRoles = context.UserRoles.Where(ur => ur.UserId == UserId);
            context.UserRoles.RemoveRange(userRoles);

            // Marcar al usuario como eliminado
            // sin borrarlo de la base de datos
            user.EmailConfirmed = false;
            user.LockoutEnabled = true;
            user.IsDeleted = true;

            await context.SaveChangesAsync();
            return true;
        }
        public async Task ReactivarUsuario(ApplicationUser user)
        {
            using var context = db.CreateDbContext();
            context.Update(user);
            await context.SaveChangesAsync();

            // Asignar rol por defecto
            var defaultRole = "Predeterminado";
            var role = await context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == defaultRole.ToUpper());
            if (role != null)
            {
                var userRole = new ApplicationUserRoles
                {
                    UserId = user.Id,
                    RoleId = role.Id
                };
                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();
            }
        }
        public async Task<bool> AlternarEstado(string Id)
        {
            using var context = db.CreateDbContext();
            var user = await context.Users.FindAsync(Id);

            if (user == null)
                return false;

            user.LockoutEnabled = !user.LockoutEnabled;
            await context.SaveChangesAsync();

            return true;
        }

    }
}
