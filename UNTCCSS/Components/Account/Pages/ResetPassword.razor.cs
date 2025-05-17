using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UNTCCSS.Components.Account;
using UNTCCSS.Data;

namespace UNTCCSS.Components.Account.Pages
{
    public partial class ResetPassword : ComponentBase
    {
        [Inject] IdentityRedirectManager RedirectManager { get; set; }
        [Inject] UserManager<ApplicationUser> UserManager { get; set; }

        private IEnumerable<IdentityError> identityErrors;

        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string Code { get; set; }

        private string Message => identityErrors is null ? null : $"Error: {string.Join(", ", identityErrors.Select(error => error.Description))}";
        protected override void OnInitialized()
        {
            if (Code is null)
            {
                RedirectManager.RedirectTo("Account/InvalidPasswordReset");
            }
            Input.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Code));

        }
        private async Task OnValidSubmitAsync()
        {
            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user is null)
            {
                // Don't reveal that the user does not exist
                RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            }

            var result = await UserManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                RedirectManager.RedirectTo("Account/ResetPasswordConfirmation");
            }

            identityErrors = result.Errors;
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "El campo es obligatorio.")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y como máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden.")]
            public string ConfirmPassword { get; set; } = "";

            [Required]
            public string Code { get; set; } = "";
        }
    }
}
