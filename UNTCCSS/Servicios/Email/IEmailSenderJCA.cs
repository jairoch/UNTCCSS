using UNTCCSS.Data;

namespace UNTCCSS.Servicios.Email
{
    public interface IEmailSenderJCA
    {
        Task<bool> SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink, string Password);
        Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink);
        Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode);
        Task SendCertificateInquiryAsync(string dni, string nombre, string mensajeCliente, string contacto);
    }
}
