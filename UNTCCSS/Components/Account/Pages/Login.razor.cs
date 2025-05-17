using UNTCCSS.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Account.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject] ILogger<Login> Logger { get; set; }
        [Inject] IAutenticacionRepositorio AutenticacionService { get; set; }
        [Inject] SignInManager<ApplicationUser> SignInManager { get; set; }

        [Inject] UserManager<ApplicationUser> UserManager { get; set; }
        [Inject] IdentityRedirectManager RedirectManager { get; set; }

        private string errorMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Eliminar la cookie externa existente para garantizar un proceso de inicio de sesión limpio
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
        }

        public async Task LoginUser()
        {
            // Normalizar el correo electrónico
            var normalizedEmail = UserManager.NormalizeEmail(Input.Email);
            var user = await UserManager.FindByEmailAsync(normalizedEmail);

            // Verificar si el usuario existe
            if (user == null || user.IsDeleted)
            {
                errorMessage = "Correo o contraseña incorrectos.";
                return;
            }

            // Verificar si la cuenta está suspendida (Agregar propiedad IsSuspended en ApplicationUser si no existe)
            if (user.LockoutEnabled)
            {
                errorMessage = "Tu cuenta ha sido suspendida. Contacta al soporte.";
                Logger?.LogWarning($"Intento de inicio de sesión en cuenta suspendida: {user.Email}");
                return;
            }

            // Verificar si el correo electrónico está confirmado
            if (!user.EmailConfirmed)
            {
                errorMessage = "Por favor, confirma tu correo electrónico antes de iniciar sesión.";
                return;
            }

            // Verificar si la contraseña es correcta
            var isPasswordCorrect = await UserManager.CheckPasswordAsync(user, Input.Password);
            if (!isPasswordCorrect)
            {
                errorMessage = "Correo o contraseña incorrectos.";
                return;
            }

            // Manejar autenticación de dos factores
            var requires2FA = await UserManager.GetTwoFactorEnabledAsync(user);
            if (requires2FA && await UserManager.GetValidTwoFactorProvidersAsync(user) is { Count: > 0 })
            {
                Logger?.LogInformation($"Usuario {user.Email} requiere autenticación de dos factores.");
                RedirectManager.RedirectTo("Account/LoginWith2fa", new()
                {
                    ["returnUrl"] = ReturnUrl,
                    ["rememberMe"] = Input.RememberMe
                });
                return;
            }

            if(AutenticacionService != null)
            {
                // Obtener los Claims del usuario (permisos)
                var claims = await AutenticacionService.GetUserClaimsAsync(user);
                var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Intentar el inicio de sesión con los Claims personalizados
                await SignInManager.SignInWithClaimsAsync(user, Input.RememberMe, claims);

                Logger?.LogInformation($"Usuario {user.Email} logeado correctamente con permisos.");
                // Redirigir siempre a /admin/home si ReturnUrl es null o vacío
                RedirectManager.RedirectTo(!string.IsNullOrEmpty(ReturnUrl) ? ReturnUrl : "/admin/home");
            }           
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
    }
}
