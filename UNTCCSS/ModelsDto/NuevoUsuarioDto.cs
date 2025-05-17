using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.ModelsDto
{
    public class NuevoUsuarioDto
    {
        [Required(ErrorMessage = "Seleccione una empresa por favor")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una empresa válida")]
        public int IdEmpresa { get; set; }

        [Required(ErrorMessage = "El DNI es Requerido")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Nombres son obligatorios")]
        public string Nombres { get; set; } = "";

        [Required(ErrorMessage = "Apellidos son obligatorios")]
        public string Apellidos { get; set; } = "";

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=(?:.*[a-z])|(?:.*[A-Z])|(?:.*\d)|(?:.*[!@#$%&*()\-_=+])){3,}.*$",
    ErrorMessage = "La contraseña debe contener al menos 3 de los siguientes: mayúscula, minúscula, número o carácter especial permitido (!@#$%&*()-_=+).")]

        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = "";


        [Required(ErrorMessage = "Teléfono obligatorio")]
        [Range(900000000, 999999999, ErrorMessage = "Ingrese un número de teléfono válido de 9 dígitos")]
        public int Telefono { get; set; }

        [Required(ErrorMessage = "Direccion requerida")]
        public string Direccion { get; set; } = "";
    }
}
