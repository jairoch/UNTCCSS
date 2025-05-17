using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security;
using UNTCCSS.Data;

namespace UNTCCSS.Models
{
    public class RolPermisos
    {
        [Key]
        public int Id { get; set; }

        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public ApplicationRole Role { get; set; }

        public int PermisosId { get; set; }
        [ForeignKey("PermisosId")]
        public Permisos Permisos { get; set; }
    }
}
