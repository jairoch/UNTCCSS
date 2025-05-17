using Newtonsoft.Json;
using UNTCCSS.Servicios.HunterIO.IHunterIO;

namespace UNTCCSS.Servicios.HunterIO
{
    public class HunterIOMaster : IHunterIOMaster
    {
        private readonly IConfiguration configuration;
        public HunterIOMaster(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<bool> validarExistenciaCorreo(string email)
        {
            var apiKey = configuration["HunterIO:ApiKey"];
            var url = $"https://api.hunter.io/v2/email-verifier?email={email}&api_key={apiKey}";
            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Lee el contenido de la respuesta JSON
                var content = await response.Content.ReadAsStringAsync();

                //// Imprime la respuesta completa en la consola
                //Console.WriteLine("Respuesta de la API:");
                //Console.WriteLine(content);

                // Deserializa el contenido en un objeto
                var result = JsonConvert.DeserializeObject<HunterEmailVerificationResponse>(content);
                if (result?.Data != null &&
                    result.Data.Score >= 60 && // Puntuación mínima de confianza
                    result.Data.SmtpCheck == true && // El servidor SMTP acepta correos para esta dirección
                    result.Data.Disposable == false && // No es un correo desechable
                    result.Data.Gibberish == false) // No es un correo sin sentido
                {
                    return true; // El correo es válido
                }
                else
                {
                    return false; // El correo no es válido
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
