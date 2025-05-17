using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UNTCCSS.Models;

namespace UNTCCSS.Data
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        public string Descripcion { get; set; }
        public bool Status {  get; set; }
        public ICollection<RolPermisos> RolPermisos { get; set; }
        public ICollection<ApplicationUserRoles> UserRoles { get; set; }
    }
}
