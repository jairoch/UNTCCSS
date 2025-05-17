using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.ModelsDto;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{
    public class RoleRepositorio : IRoleRepositorio
    {
        private readonly IDbContextFactory<ApplicationDbContext> db;
        private readonly UserManager<ApplicationUser> userManager;
        public RoleRepositorio(IDbContextFactory<ApplicationDbContext> db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        public async Task<List<RolResumenDto>> GetResumenRolesAsync()
        {
            using var context = db.CreateDbContext();

            var roles = await context.Roles
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .Include(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permisos)
                .ToListAsync();

            var resumen = roles.Select(rol => new RolResumenDto
            {
                NombreRol = rol.Name,
                CantidadUsuarios = rol.UserRoles.Count,
                UsuariosActivos = rol.UserRoles.Count(ur => !ur.User.LockoutEnabled),
                UsuariosSuspendidos = rol.UserRoles.Count(ur => ur.User.LockoutEnabled),
                Permisos = rol.RolPermisos.Select(rp => rp.Permisos.Name).Distinct().ToList()
            }).ToList();

            return resumen;
        }
        public async Task<int> ObtenerTotalRolesAsync()
        {
            using var context = db.CreateDbContext();
            int totalRoles = await context.Roles.CountAsync();
            int totalPaginas = (int)Math.Ceiling((double)totalRoles / 10);
            return totalPaginas;
        }
        public async Task<List<ApplicationRole>> ListaRolesPaginadosAsync(int pagina)
        {
            using var context = db.CreateDbContext();

            int omitir = (pagina - 1) * 10;

            var roles = await context.Roles
                .Include(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permisos)
                .OrderBy(r => r.Name)
                .Skip(omitir)       
                .Take(10)
                .ToListAsync();

            return roles;
        }
        public async Task<List<Permisos>> ListaPermisos()
        {
            using var context = db.CreateDbContext();
            return await context.Permisos.ToListAsync();
        }
        public async Task<ResponseSaveRole> CrearRol(ApplicationRole nuevoRol)
        {
            using var context = db.CreateDbContext();

            bool rolExiste = await context.Roles
                .AnyAsync(r => r.Name.Trim().ToLower() == nuevoRol.Name.ToLower());

            if (rolExiste)
            {
                return new ResponseSaveRole
                {
                    Role = null,
                    Message = "Error: Este rol ya se encuentra registrado. Por favor, elija otro nombre."
                };
            }
            nuevoRol.ConcurrencyStamp = Guid.NewGuid().ToString();
            context.Roles.Add(nuevoRol);
            await context.SaveChangesAsync();

            return new ResponseSaveRole
            {
                Role = nuevoRol,
                Message = "Success: El rol se ha creado exitosamente."
            };
        }
        public async Task AsignarPermisosARol(List<RolPermisos> rolPermisos)
        {
            //Agregamos Permisos a un Rol
            using var context = db.CreateDbContext();
            context.RolPermisos.AddRange(rolPermisos);
            await context.SaveChangesAsync();
        }
        public async Task<ApplicationRole> ObtenerRolPorIdAsync(string roleId)
        {
            if(roleId != null)
            {
                using var context = db.CreateDbContext();
                return await context.Roles
                    .Include(r => r.RolPermisos)
                    .ThenInclude(rp => rp.Permisos)
                    .FirstOrDefaultAsync(r => r.Id == roleId);
            }
            return null;
        }
        public async Task<Permisos> ObtenerPermisoPorIdAsync(int permisoId)
        {
            using var context = db.CreateDbContext();
            return await context.Permisos.FindAsync(permisoId);
        }
        public async Task QuitarPermisoDeRol(RolPermisos rolPermiso)
        {
            using var context = db.CreateDbContext();
            context.RolPermisos.Remove(rolPermiso);
            await context.SaveChangesAsync();
        }
        public async Task<bool> EliminarRolAsync(string roleId)
        {
            using var context = db.CreateDbContext();
            var rol = await context.Roles
                .Include(r => r.RolPermisos)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (rol == null || rol.Name == "Predeterminado")
                return false;

            context.RolPermisos.RemoveRange(rol.RolPermisos); 
            context.Roles.Remove(rol); 
            await context.SaveChangesAsync();
            return true; 
        }
    }
}
