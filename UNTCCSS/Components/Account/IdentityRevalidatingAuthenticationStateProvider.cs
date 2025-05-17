using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Account
{
    // This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
    // every 30 minutes an interactive circuit is connected.
    internal sealed class IdentityRevalidatingAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> options)
        : RevalidatingServerAuthenticationStateProvider(loggerFactory)
    {
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            await using var scope = scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            bool isValid = await ValidateSecurityStampAsync(userManager, authenticationState.User);
            if (!isValid)
            {
                return false;
            }
            //USUARIO VALIDADO
            var user = await userManager.GetUserAsync(authenticationState.User);
            if (user != null)
            {
                var authRepo = scope.ServiceProvider.GetRequiredService<IAutenticacionRepositorio>();
                var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
                var nuevosClaims = await authRepo.GetUserClaimsAsync(user);
                await signInManager.SignInWithClaimsAsync(user, isPersistent: false, nuevosClaims);
            }
            return true;
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
