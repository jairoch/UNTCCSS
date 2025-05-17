using UNTCCSS.Servicios.JCASoftware.JCAReponses;

namespace UNTCCSS.Servicios.JCASoftware
{
    public interface IJCARepositorio
    {
        Task<ResponseConsultaDni> ObtenerPersona(string Dni);
    }
}
