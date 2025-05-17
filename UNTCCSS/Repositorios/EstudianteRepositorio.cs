using UNTCCSS.ModelsDto;
using Microsoft.EntityFrameworkCore;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{
    public class EstudianteRepositorio : IEstudianteRepositorio
    {
        private readonly IDbContextFactory<ApplicationDbContext> db;
        public EstudianteRepositorio(IDbContextFactory<ApplicationDbContext> db)
        {
            this.db = db;
        }
        public async Task<Estudiante> BuscarEstudiante(string Dni)
        {
            using var context = db.CreateDbContext();
            return await context.Estudiantes
                                .FirstOrDefaultAsync(e => e.DNI == Dni);
        }
        public async Task<Estudiante> RegistrarEstudiante(Estudiante estudiante)
        {
            using var context = db.CreateDbContext();
            estudiante.DNI = estudiante.DNI.Trim();
            bool existe = await context.Estudiantes.AnyAsync(e => e.DNI == estudiante.DNI);

            if (existe)
            {
                return null;
            }

            estudiante.Estado = true;
            estudiante.IsDelete = false;
            context.Estudiantes.Add(estudiante);
            await context.SaveChangesAsync();

            return estudiante;
        }
        public async Task<bool> UpdateEstudiante(Estudiante estudiante)
        {
            using var context = db.CreateDbContext();
            context.Update(estudiante);
            await context.SaveChangesAsync(); 
            return true;
        }

        public async Task<Estudiante> MisCursos(int EstudianteId)
        {
            using var context = db.CreateDbContext();
            return await context.Estudiantes
                .Where(d => d.Id == EstudianteId)
                .Select(d => new Estudiante
                {
                    Id = d.Id,
                    Nombres = d.Nombres,
                    Apellidos = d.Apellidos,
                    DNI = d.DNI,
                    Estado = d.Estado,
                    Certificados = d.Certificados
                        .Where(c => c.Status == true)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ValidarEstudiante(string DNI)
        {
            using var context = db.CreateDbContext();
            return await context.Estudiantes
                .AnyAsync(e => e.DNI == DNI.Trim() && e.Estado == true);
        }
        public async Task<bool> ValidarCursosEstudiante(string DNI)
        {
            using var context = db.CreateDbContext();
            return await context.Estudiantes
                .Where(e => e.DNI == DNI.Trim() && e.Estado == true)
                .AnyAsync(e => e.Certificados.Any(c => c.Status == true));
        }
        public async Task<int> ObtenerIdEstudiante(string DNI)
        {
            using var context = db.CreateDbContext();
            return await context.Estudiantes
                .Where(e => e.DNI == DNI.Trim())
                .Select(e => e.Id)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> Delete(int IdEstudiante)
        {
            try
            {
                using var context = db.CreateDbContext();

                // 📌 Buscar al estudiante BD
                var estudiante = await context.Estudiantes
                    .Include(e => e.Certificados)
                    .FirstOrDefaultAsync(e => e.Id == IdEstudiante);

                if (estudiante == null)
                {
                    return false;
                }

                // 📌 Desactivar todos los certificados
                // asociados a este estudiante
                foreach (var cert in estudiante.Certificados)
                {
                    cert.Status = false;
                    cert.IsDelete = true;
                }

                // 📌 Marcar al estudiante
                // como inactivo y eliminado
                estudiante.Estado = false;
                estudiante.IsDelete = true;

                // 📌 Guardar cambios en la BD
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
