using Microsoft.EntityFrameworkCore;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{
    public class ResolucionRepositorio : IResolucionRepositorio
    {
        private readonly IDbContextFactory<ApplicationDbContext> db;
        public ResolucionRepositorio(IDbContextFactory<ApplicationDbContext> db)
        {
            this.db = db;
        }
        public async Task<List<Resolucion>> Resoluciones()
        {
            using var context = db.CreateDbContext();
            return await context.Resolucion.ToListAsync();
        }
    }
}
