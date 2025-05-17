using UNTCCSS.Models;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface ICursoRepositorio
    {
        Task<List<Curso>> ListaCursos();
        Task<bool> ExisteCurso(int? Id);
    }
}
