using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UNTCCSS.Data;

namespace UNTCCSS.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        //[EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string Email { get; set; } = string.Empty;

        //[StringLength(9, MinimumLength = 9, ErrorMessage = "El teléfono debe tener exactamente 9 dígitos.")]
        //[RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe contener solo números.")]
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public bool Estado { get; set; } = false;
        public bool IsDelete { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser Usuario { get; set; }
        public DateTime AtCreated { get; set; }

        public List<Certificado> Certificados { get; set; }
    }
}
