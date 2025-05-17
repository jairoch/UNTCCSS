using System.ComponentModel.DataAnnotations;

namespace UNTCCSS.Models
{
    public class Permisos
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Status { get; set; }
        public string Descripcion { get; set; }
        public ICollection<RolPermisos> RolePermissions { get; set; }
    }
}
