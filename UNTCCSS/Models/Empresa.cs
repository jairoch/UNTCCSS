namespace UNTCCSS.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required][MaxLength(15)]
        public string RUC { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required][MaxLength(100)]
        public string RazonSocial { get; set; }

        [MaxLength(20)] 
        public string Telefono { get; set; } 
        
        [MaxLength(200)]
        public string Direccion { get; set; }
    }
}