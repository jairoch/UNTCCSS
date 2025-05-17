using Microsoft.EntityFrameworkCore;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{
    public class CursoRepositorio : ICursoRepositorio
    {
        private readonly IDbContextFactory<ApplicationDbContext> db;
        public CursoRepositorio(IDbContextFactory<ApplicationDbContext> db)
        {
            this.db = db;
        }
        public async Task<List<Curso>> ListaCursos()
        {
            using var context = db.CreateDbContext();
            return await context.Curso.ToListAsync();
        }
        public async Task<bool> ExisteCurso(int? Id)
        {
            if(Id == null) return false;
            using var context = db.CreateDbContext();
            return await context.Curso.AnyAsync(c => c.Id == Id);
        }
    }
}
