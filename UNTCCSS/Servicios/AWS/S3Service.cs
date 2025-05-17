using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace UNTCCSS.Servicios.AWS
{
    public class S3Service : IS3Service
    {
        private readonly string bucketName;
        private readonly IAmazonS3 s3Client;
        public S3Service(IConfiguration configuration)
        {
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var region = configuration["AWS:Region"];

            bucketName = configuration["AWS:BucketName"];
            s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
        }
        public async Task<bool> SubirArchivoAsync(Stream archivoStream, string dni, string key)
        {
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = archivoStream,
                    Key = key,
                    BucketName = bucketName,
                    ContentType = "application/pdf"
                };

                using var transferUtility = new TransferUtility(s3Client);
                await transferUtility.UploadAsync(uploadRequest);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir el archivo: {ex.Message}");
                return false;
            }
        }
        public async Task<Stream> DownloadCertificadosAsync(string key, string Dni)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                var response = await s3Client.GetObjectAsync(request);
                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                string contentType = response.Headers["Content-Type"];
                return memoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mensaje de error" + ex.Message );
                return null;
            }
        }

        public async Task<bool> EliminarArchivoAsync(string dni)
        {
            try
            {
                string key = $"Certificados/{dni}/archivos.pdf";
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                await s3Client.DeleteObjectAsync(deleteRequest);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el archivo: {ex.Message}");
                return false;
            }
        }
    }
}
