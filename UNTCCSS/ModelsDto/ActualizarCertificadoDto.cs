using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.ModelsDto
{
    public class ActualizarCertificadoDto
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public int IdResolucion { get; set; }
        [Required]
        public string TipoDocumento {  get; set; }
        public int PromedioFinal {  get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public DateTime FechaEmision { get; set; }
    }
}
