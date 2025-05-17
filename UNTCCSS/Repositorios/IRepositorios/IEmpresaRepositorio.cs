using System.Threading.Tasks;
using UNTCCSS.ModelsDto;

namespace UNTCCSS.Repositorios.IRepositorios
{
    public interface IEmpresaRepositorio
    {
        Task<List<ComboBox>> ListaEmpresas();
        Task<List<ComboBox>> ObtenerEmpresaPorId(int empresaId);
    }
}
