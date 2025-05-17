using UNTCCSS.Components.Pages.Admin;
using UNTCCSS.ModelsDto;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{   
    public class CertificadoRepositorio : ICertificadoRepositorio
    {
        private readonly IDbContextFactory<ApplicationDbContext> db;
        private readonly IUsersRepositorio usersRepositorio;
        public CertificadoRepositorio(IDbContextFactory<ApplicationDbContext> db, IUsersRepositorio usersRepositorio)
        {
            this.db = db;
            this.usersRepositorio = usersRepositorio;
        }
        public async Task<bool> RegistrarCertificado(Certificado certificado)
        {
            try
            {
                using var context = db.CreateDbContext();
                var estudiante = await context.Estudiantes.FindAsync(certificado.AlumnoId);
                //Activar estudiante si esta eliminado o desactivado
                if (estudiante != null)
                {
                    if (!estudiante.Estado)
                    {
                        estudiante.Estado = true;
                    }
                    if (estudiante.IsDelete)
                    {
                        estudiante.IsDelete = false;
                    }
                }

                // 📌 Registrar el certificado
                certificado.RegCreated = DateTime.Now;
                certificado.RegUpdated = DateTime.Now;
                certificado.Status = true;
                certificado.IsDelete = false;

                context.Add(certificado);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al registrar certificado: {ex.Message}");
                return false;
            }
        }
        public int TotalPages { get; private set; }
        public async Task<List<Certificado>> ListarTodos(DateTime inicio, DateTime fin, int pagina)
        {
            const int registrosPorPagina = 15;
            using var context = db.CreateDbContext();

            // Calcular el total de registros
            var totalRegistros = await context.Certificado
                .Where(c => !c.IsDelete &&
                            c.RegCreated.Date >= inicio.Date &&
                            c.RegCreated.Date <= fin.Date)
                .CountAsync();

            // Calcular el total de páginas
            TotalPages = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            // Obtener los registros paginados
            var certificados = await context.Certificado
                .Where(c => !c.IsDelete &&
                            c.RegCreated.Date >= inicio.Date &&
                            c.RegCreated.Date <= fin.Date)
                .Include(e => e.Alumno)
                    .ThenInclude(c => c.Usuario)
                .OrderByDescending(c => c.FechaEmision)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            return certificados;
        }

        public async Task<List<Certificado>> ListarFiltrado(DateTime inicio, DateTime fin, int pagina)
        {
            const int registrosPorPagina = 15;

            var user = await usersRepositorio.GetUserWithProfileAsync();
            if (user?.IdEmpresa != null)
            {
                using var context = db.CreateDbContext();

                // Calcular el total de registros
                var totalRegistros = await context.Certificado
                    .Where(c => !c.IsDelete &&
                                c.RegCreated.Date >= inicio.Date && c.RegCreated.Date <= fin.Date &&
                                context.Users
                                    .Where(u => u.IdEmpresa == user.IdEmpresa)
                                    .Select(u => u.Id)
                                    .Contains(c.Usuario.Id))
                    .CountAsync();

                // Calcular el total de páginas
                TotalPages = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

                // Obtener registros paginados
                var certificados = await context.Certificado
                    .Where(c => !c.IsDelete &&
                                c.RegCreated.Date >= inicio.Date && c.RegCreated.Date <= fin.Date &&
                                context.Users
                                    .Where(u => u.IdEmpresa == user.IdEmpresa)
                                    .Select(u => u.Id)
                                    .Contains(c.Usuario.Id))
                    .Include(c => c.Alumno)
                    .OrderByDescending(c => c.RegCreated)
                    .Skip((pagina - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToListAsync();

                return certificados;
            }

            return new List<Certificado>();
        }
        public async Task<List<Certificado>> FiltrarTodosPorDniAdmin(string dni)
        {
            using var context = db.CreateDbContext();

            return await context.Certificado
                .Where(c => !c.IsDelete && c.Alumno.DNI == dni)
                .Include(e => e.Alumno)
                    .ThenInclude(c => c.Usuario)
                .ToListAsync();
        }

        public async Task<List<Certificado>> FiltrarTodosPorDniEmpresa(string dni)
        {
            var user = await usersRepositorio.GetUserWithProfileAsync();

            if (user?.IdEmpresa != null)
            {
                using var context = db.CreateDbContext();

                return await context.Certificado
                    .Where(c => !c.IsDelete &&
                                c.Alumno.DNI == dni &&
                                context.Users
                                    .Where(u => u.IdEmpresa == user.IdEmpresa)
                                    .Select(u => u.Id)
                                    .Contains(c.Usuario.Id))
                    .Include(e => e.Alumno)
                        .ThenInclude(c => c.Usuario)
                    .ToListAsync();
            }

            return new List<Certificado>();
        }


        public async Task<string> ComprobarArchivo(string Registro, string Codigo)
        {
            using var context = db.CreateDbContext();

            var certificado = await context.Certificado
                .Where(c => c.Registro == Registro.Trim() && c.Codigo == Codigo.Trim() && !string.IsNullOrEmpty(c.Archivo))
                .Select(c => c.Archivo)
                .FirstOrDefaultAsync();

            return certificado;
        }
        public async void InsertarNombreArchivo(int IdCertificado, string key)
        {
            using var context = db.CreateDbContext();
            var certificado = await context.Certificado.FindAsync(IdCertificado);
            if (certificado != null)
            {
                certificado.Archivo = key;
                await context.SaveChangesAsync();
            }           
        }
        public async Task<Certificado> OptenerCertificado(int CertificadoId)
        {
            using var context = db.CreateDbContext();
            return await context.Certificado.FindAsync(CertificadoId);
        }
        public async Task<bool> ActualizarCertificado(Certificado certificado)
        {
            using var context = db.CreateDbContext();

            var certificadoExistente = await context.Certificado.FindAsync(certificado.Id);
            if (certificadoExistente != null)
            {
                context.Entry(certificadoExistente).CurrentValues.SetValues(certificado);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> PausarVerificacion(int IdCertificado)
        {
            using var context = db.CreateDbContext();
            var certificado = await context.Certificado.FindAsync(IdCertificado);
            if(certificado != null)
            {
                certificado.Status = false;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> ReanudarVerificacion(int IdCertificado)
        {
            using var context = db.CreateDbContext();
            var certificado = await context.Certificado.FindAsync(IdCertificado);
            if (certificado != null)
            {
                certificado.Status = true;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> Delete(int IdCertificado)
        {
            using var context = db.CreateDbContext();
            var certificado = await context.Certificado.FindAsync(IdCertificado);
            if (certificado != null)
            {
                certificado.Status = false;
                certificado.IsDelete = true;
                await context.SaveChangesAsync();
                return true;
            }
            return false;

        }
    }
}
