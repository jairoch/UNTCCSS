using Newtonsoft.Json;

namespace UNTCCSS.Servicios.HunterIO
{
    public class HunterEmailVerificationResponse
    {
        [JsonProperty("data")]
        public HunterEmailVerificationData Data { get; set; }
    }
}
