using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.Models
{
    public class Resolucion
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Documentacion { get; set; }
        public bool Status { get; set; }
        public DateTime AtCreated { get; set; }
        public ICollection<Certificado> Certificados { get; set; }
    }
}
