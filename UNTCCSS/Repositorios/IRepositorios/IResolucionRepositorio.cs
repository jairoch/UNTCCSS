using UNTCCSS.Models;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface IResolucionRepositorio
    {
        Task<List<Resolucion>> Resoluciones();
    }
}
