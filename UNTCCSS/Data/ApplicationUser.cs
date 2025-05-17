using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using UNTCCSS.Models;

namespace UNTCCSS.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int? IdEmpresa { get; set; }
        [ForeignKey("IdEmpresa")]
        public Empresa Empresa { get; set; }

        public int PerfilId {  get; set; }
        [ForeignKey("PerfilId")]
        public Perfil Perfil { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<ApplicationUserRoles> UserRoles { get; set; }
        public ICollection<Certificado> Certificados { get; set; }
    }

}
