namespace UNTCCSS.Servicios.HunterIO.IHunterIO
{
    public interface IHunterIOMaster
    {
        Task<bool> validarExistenciaCorreo(string email);
    }
}
