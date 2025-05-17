using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.Models
{
    public class Temario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; }
    }
}
