using Microsoft.AspNetCore.Identity;

namespace UNTCCSS.Data
{
    public class ApplicationUserRoles : IdentityUserRole<string>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
