using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Account.Pages
{
    public partial class ConfirmEmail : ComponentBase
    {
        [Inject] IAutenticacionRepositorio autenticacionRepositorio {  get; set; }
        public ConfirmEmail()
        {
            
        }
        private string statusMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string UserId { get; set; }

        [SupplyParameterFromQuery]
        private string Token { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (UserId is null || Token is null)
            {
                RedirectManager.RedirectTo("");
            }

            var user = await UserManager.FindByIdAsync(UserId);
            if (user is null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                statusMessage = $"Error loading user with ID {UserId}";
            }
            else
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Token));
                var result = await autenticacionRepositorio.ConfirmarEmailAsync(user, code);
                //var result = await UserManager.ConfirmEmailAsync(user, code);
                statusMessage = result.Succeeded ? "Gracias por confirmar su correo electrónico." : "Error al confirmar tu correo electrónico.";
            }
        }
    }
}
