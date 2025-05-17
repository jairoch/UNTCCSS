using Newtonsoft.Json;

namespace UNTCCSS.Servicios.HunterIO
{
    public class HunterEmailVerificationData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; } // Valores posibles: "deliverable", "undeliverable", "risky"

        [JsonProperty("score")]
        public int Score { get; set; } // Puntuación de confianza en la validez del correo

        [JsonProperty("regexp")]
        public bool Regexp { get; set; } // Si el correo cumple con el formato de expresión regular

        [JsonProperty("gibberish")]
        public bool Gibberish { get; set; } // Si el correo parece ser un texto sin sentido

        [JsonProperty("disposable")]
        public bool Disposable { get; set; } // Si el correo es desechable (como mailinator)

        [JsonProperty("webmail")]
        public bool Webmail { get; set; } // Si el correo pertenece a servicios como Gmail, Yahoo, etc.

        [JsonProperty("mx_records")]
        public bool MxRecords { get; set; } // Si hay registros MX válidos para el dominio

        [JsonProperty("smtp_server")]
        public bool SmtpServer { get; set; } // Si hay un servidor SMTP accesible

        [JsonProperty("smtp_check")]
        public bool SmtpCheck { get; set; } // Si el servidor SMTP acepta correos para este correo

        [JsonProperty("accept_all")]
        public bool AcceptAll { get; set; } // Si el dominio acepta cualquier correo
    }
}
