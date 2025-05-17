using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UNTCCSS.Models;

namespace UNTCCSS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRoles, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        //Agregamos los modelos
        public DbSet<Certificado> Certificado { get; set; }
        public DbSet<Resolucion> Resolucion { get; set; }
        public DbSet<Curso> Curso { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Perfil> Perfil { get; set; }
        public DbSet<Permisos> Permisos { get; set; }
        public DbSet<RolPermisos> RolPermisos { get; set; }
        public DbSet<Temario> Temario { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Solo PostgresSQL
            /*Estás diciéndole a PostgreSQL que en vez de usar datetime
            * (que en PostgreSQL sería timestamp with time zone por defecto),
            * use timestamp without time zone, es decir, un timestamp simple
            * que no tiene información de zona horaria.*/

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetColumnType("timestamp without time zone");
                    }
                }
            }

            base.OnModelCreating(modelBuilder);

            Configuracion(modelBuilder);

            SeedData(modelBuilder);
        }

        private void Configuracion(ModelBuilder modelBuilder)
        {
            // Configuración personalizada de ApplicationUserRoles
            modelBuilder.Entity<ApplicationUserRoles>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId }); // Clave compuesta
                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.Id)
                .HasColumnOrder(1);

            modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.Name)
                .HasColumnOrder(2);

            modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.NormalizedName)
                .HasColumnOrder(3);

            modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.ConcurrencyStamp)
                .HasColumnOrder(4);

            modelBuilder.Entity<ApplicationRole>()
                .Property(r => r.Descripcion)
                .HasColumnOrder(5);

            //Eliminar un Certificado => No eliminar Alumno
            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Alumno)
                .WithMany(e => e.Certificados)
                .HasForeignKey(c => c.AlumnoId)
                .OnDelete(DeleteBehavior.NoAction);

            //Eliminar un Certificado => No eliminar un Usuario
            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Certificados)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            //Eliminar un Certificado => No eliminar una Resolución
            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Resolucion)
                .WithMany(r => r.Certificados)
                .HasForeignKey(c => c.IdResolucion)
                .OnDelete(DeleteBehavior.NoAction);

            //Eliminar un Certificado => No eliminar un Curso
            modelBuilder.Entity<Certificado>()
                .HasOne(c => c.Curso)
                .WithMany(c => c.Certificados)
                .HasForeignKey(c => c.CursoId)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            SeedFromJson<Resolucion>(modelBuilder, "Data/JSON/Resoluciones.json");
            SeedFromJson<ApplicationRole>(modelBuilder, "Data/JSON/Roles.json");
            SeedFromJson<Perfil>(modelBuilder, "Data/JSON/Perfil.json");
            SeedFromJson<Empresa>(modelBuilder, "Data/JSON/Empresa.json");
            SeedFromJson<ApplicationUser>(modelBuilder, "Data/JSON/User.json");
            SeedFromJson<Permisos>(modelBuilder, "Data/JSON/Permisos.json");
            SeedFromJson<RolPermisos>(modelBuilder, "Data/JSON/RolPermisos.json");
            SeedFromJson<ApplicationUserRoles>(modelBuilder, "Data/JSON/UsersRole.json"); // Cambiado a ApplicationUserRoles
        }

        private void SeedFromJson<TEntity>(ModelBuilder modelBuilder, string filePath) where TEntity : class
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var jsonData = File.ReadAllText(filePath);
            var entities = JsonConvert.DeserializeObject<List<TEntity>>(jsonData);

            if (entities == null || !entities.Any())
                throw new InvalidOperationException($"No data found in {filePath}");

            modelBuilder.Entity<TEntity>().HasData(entities);
        }
    }
}
