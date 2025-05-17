using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UNTCCSS.Data;
using UNTCCSS.ModelsDto;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Repositorios
{
    public class EmpresasRepositorio : IEmpresaRepositorio
    {
        private readonly IDbContextFactory<ApplicationDbContext> db;
        private readonly UserManager<ApplicationUser> userManager;
        public EmpresasRepositorio(IDbContextFactory<ApplicationDbContext> db, UserManager<ApplicationUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        //Metodo solo para Rol Admin y Usuarios con permisos GestionarUsuarios
        public async Task<List<ComboBox>> ListaEmpresas()
        {
            using var context = db.CreateDbContext();

            var empresas = await context.Empresa.ToListAsync();

            var lista = empresas.Select(e => new ComboBox
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Descripcion = ""
            }).ToList();

            return lista;
        }
        //Metodo solo para Rol AdminRegistrador y Usuarios con permisos GestionarUsuarios
        public async Task<List<ComboBox>> ObtenerEmpresaPorId(int empresaId)
        {
            using var context = db.CreateDbContext();

            var empresa = await context.Empresa
                .Where(e => e.Id == empresaId)
                .Select(e => new ComboBox
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Descripcion = ""
                })
                .ToListAsync();

            return empresa;
        }

    }
}
