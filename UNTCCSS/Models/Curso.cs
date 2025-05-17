using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNTCCSS.Models
{
    public class Curso
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; }

        public int? IdTemario { get; set; }
        [ForeignKey("IdTemario")]
        public Temario Temario { get; set; }

        public ICollection<Certificado> Certificados { get; set; }
    }
}
