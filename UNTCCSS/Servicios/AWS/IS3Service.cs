namespace UNTCCSS.Servicios.AWS
{
    public interface IS3Service
    {
        Task<bool> SubirArchivoAsync(Stream archivoStream, string dni, string key);
        Task<bool> EliminarArchivoAsync(string dni);
        Task<Stream> DownloadCertificadosAsync(string key, string Dni);
    }
}
