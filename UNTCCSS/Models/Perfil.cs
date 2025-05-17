using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.Models
{
    public class Perfil
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Dni {  get; set; } = string.Empty;

        [Required]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        public int Telefono { get; set; }

        [Required]
        public string Email {  get; set; } = string.Empty;

        [Required]
        public string Direccion { get; set; } = string.Empty;

        public string ImagenPerfil { get; set; }

    }
}
