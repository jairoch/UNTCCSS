namespace UNTCCSS.Servicios.JCASoftware.JCAReponses
{
    public class ResponseConsultaDni
    {
        public bool Success { get; set; }
        public string Dni { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public int CodVerifica { get; set; }
        public  string CodVerificaLetra { get; set; } = string.Empty;

    }
}
