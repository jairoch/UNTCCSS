using UNTCCSS.Models;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface IEstudianteRepositorio
    {
        Task<Estudiante> BuscarEstudiante(string Dni);
        Task<Estudiante> RegistrarEstudiante(Estudiante estudiante);
        Task<Estudiante> MisCursos(int EstudianteId);
        Task<bool> ValidarCursosEstudiante(string DNI);
        Task<int> ObtenerIdEstudiante(string DNI);
        Task<bool> UpdateEstudiante(Estudiante estudiante);
        Task<bool> Delete(int IdCertificado);
    }
}
