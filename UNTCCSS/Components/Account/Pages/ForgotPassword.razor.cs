using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UNTCCSS.Servicios.Email;

namespace UNTCCSS.Components.Account.Pages
{
    public partial class ForgotPassword
    {
        [SupplyParameterFromForm]
        private InputModel Input { get; set; } = new();
        [Inject] IEmailSenderJCA emailServices { get; set; }
        string errorMessage;

        private async Task OnValidSubmitAsync()
        {
            errorMessage = string.Empty;
            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user is null || !(await UserManager.IsEmailConfirmedAsync(user)))
            {
                // No revelar que el usuario no existe o no está confirmado
                RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
                return;
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("account/resetPassword").AbsoluteUri,
                new Dictionary<string, object> { ["code"] = code });

            // ✅ Pasamos el User, Email y Url
            await emailServices.SendPasswordResetLinkAsync(user, Input.Email, callbackUrl);

            RedirectManager.RedirectTo("Account/ForgotPasswordConfirmation");
        }

        private sealed class InputModel
        {
            [Required(ErrorMessage = "Su correo es requerido")]
            [EmailAddress]
            public string Email { get; set; } = "";
        }
    }
}
