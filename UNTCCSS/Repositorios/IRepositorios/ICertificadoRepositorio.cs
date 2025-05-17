using System.Threading.Tasks;
using UNTCCSS.Models;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface ICertificadoRepositorio
    {
        Task<bool> RegistrarCertificado(Certificado certificado);
        public int TotalPages { get; }
        Task<List<Certificado>> ListarTodos(DateTime inicio, DateTime fin, int pagina);
        Task<List<Certificado>> ListarFiltrado(DateTime inicio, DateTime fin, int pagina);
        Task<string> ComprobarArchivo(string Registro, string Codigo);
        void InsertarNombreArchivo(int IdCertificado, string key);
        Task<Certificado> OptenerCertificado(int CertificadoId);
        Task<bool> ActualizarCertificado(Certificado certificado);
        Task<bool> PausarVerificacion(int IdCertificado);
        Task<bool> ReanudarVerificacion(int IdCertificado);
        Task<List<Certificado>> FiltrarTodosPorDniAdmin(string dni);
        Task<List<Certificado>> FiltrarTodosPorDniEmpresa(string dni);
        Task<bool> Delete(int IdCertificado);
    }
}
