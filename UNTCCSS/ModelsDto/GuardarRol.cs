using UNTCCSS.Models;
using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.ModelsDto
{
    public class GuardarRol
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción del rol es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar al menos un permiso.")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos un permiso.")]
        public List<int> PermisosSeleccionados { get; set; } = new();
    }
}
