using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.Reflection;
using UNTCCSS.Data;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Servicios.Email
{
    public class EmailSender : IEmailSenderJCA
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;
        private readonly IUsersRepositorio usersService;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger, IUsersRepositorio usersService)
        {
            _configuration = configuration;
            _logger = logger;
            this.usersService = usersService;
        }

        /// <summary>
        /// Envío genérico de correo.
        /// </summary>
        public async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_configuration["Email:UserName"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;
                message.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["Email:Host"],
                    Convert.ToInt32(_configuration["Email:Port"]),
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    _configuration["Email:UserName"],
                    _configuration["Email:PassWord"]);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo");
                return false;
            }
        }

        /// <summary>
        /// Enviar correo de confirmación de cuenta.
        /// </summary>
        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink, string Password)
        {
            var perfil = await usersService.GetProfile(user);
            if (perfil != null)
            {
                string subject = "Confirma tu cuenta";
                string message = $"<p>Hola {perfil.Nombres},</p>" +
                                 $"<p>Por favor, confirma tu cuenta haciendo clic en el siguiente enlace:</p>" +
                                  $"<p><a href='{confirmationLink}'>Confirmar cuenta</a></p>";

                await SendEmailAsync(email, subject, message);
            }
        }

        /// <summary>
        /// Enviar correo de restablecimiento de contraseña.
        /// </summary>
        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            string subject = "Restablecimiento de contraseña";
            string message = await LoadHtmlTemplate(user, resetLink);

            await SendEmailAsync(email, subject, message);
        }

        /// <summary>
        /// Enviar código de restablecimiento de contraseña (opcional).
        /// </summary>
        public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            var perfil = await usersService.GetProfile(user);
            if (perfil != null)
            {
                string subject = "Código de restablecimiento de contraseña";
                string message = $"<p>Hola {perfil.Nombres},</p>" +
                                 $"<p>Usa el siguiente código para restablecer tu contraseña:</p>" +
                                 $"<h2>{resetCode}</h2>" +
                                 $"<p>Si no solicitaste este cambio, ignora este correo.</p>";

                await SendEmailAsync(email, subject, message);
            }
        }

        public async Task SendCertificateInquiryAsync(string dni, string nombre, string mensajeCliente, string contacto)
        {
            string subject = "Consulta sobre Certificados";

            string message = $@"
<div style='font-family: Arial, sans-serif; color: #333; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px; margin: auto;'>
    <h3 style='color: #2230B0; text-align: center;'>Consulta de Certificados</h3>
    <p><strong>Nombre:</strong> {nombre}</p>
    <p><strong>DNI:</strong> {dni}</p>
    <p><strong>Contacto:</strong> {contacto}</p>
    <p><strong>Mensaje:</strong></p>
    <blockquote style='background: #f4f4f4; padding: 10px; border-left: 4px solid #2230B0;'>{mensajeCliente}</blockquote>
    <p style='text-align: center; font-size: 14px; color: #777;'>Este mensaje fue enviado desde la plataforma de consulta de certificados.</p>
</div>";

            await SendEmailAsync("admin@massaben.com", subject, message);
        }



        /// <summary>
        /// Cargar plantilla HTML personalizada para recuperación de contraseña.
        /// </summary>
        private async Task<string> LoadHtmlTemplate(ApplicationUser user, string callbackUrl)
        {
            var perfil = await usersService.GetProfile(user);
            if (perfil != null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "UNTCCSS.Servicios.Email.Templates.RecuperacionContraseña.html";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string mensaje = reader.ReadToEnd();
                    mensaje = mensaje.Replace("{{Nombre}}", perfil.Nombres);
                    mensaje = mensaje.Replace("{{Link}}", callbackUrl);
                    return mensaje;
                }
            }
            return "Lamentamos los inconvenientes no cuentas con un perfil";
        }
    }
}
