using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.JSInterop;
using Org.BouncyCastle.Bcpg;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;
using UNTCCSS.Servicios.AWS;
using UNTCCSS.Servicios.Email;

namespace UNTCCSS.Components.Pages
{
    public partial class ConsultaExitosa : ComponentBase
    {
        [Parameter] public string Token { get; set; }
        [Inject] IEstudianteRepositorio estudianteRepositorio {  get; set; }
        [Inject] IAutenticacionRepositorio Autenticacion {  get; set; }
        [Inject] IEmailSenderJCA ServiceEmail {  get; set; }
        [Inject] IS3Service S3 {  get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NavigationManager navigationManager { get; set; }
        private string MensajeEmail {  get; set; } = string.Empty;
        private string ContactoUsuario { get; set; } = string.Empty;

        private Estudiante estudiante = new();
        private List<Certificado> Certificados = new();
        public ConsultaExitosa()
        {
            
        }
        private string NombreCompleto;
        protected override async Task OnInitializedAsync()
        {
            int estudianteId = Autenticacion.DecodificarTokenConsultaCert(Token);
            if(estudianteId != 0)
            {
                var estudianteResponse = await estudianteRepositorio.MisCursos(estudianteId);
                if (estudianteResponse != null)
                {
                    estudiante = estudianteResponse;
                    NombreCompleto = estudiante.Nombres + " " + estudiante.Apellidos;
                    if(estudiante.Certificados != null && estudiante.Certificados.Any())
                    {
                        Certificados = estudiante.Certificados.ToList();
                    }
                    else
                    {
                        navigationManager.NavigateTo("/", true);
                    }
                }
            }
            else
            {
                navigationManager.NavigateTo("/", true);
            }
        }

        private bool MostrarCorreo = false;
        private void MostrarFormularioCorreo() => MostrarCorreo = true;
        private void CerrarModalCorreo() => MostrarCorreo = false;
        private async void SendEmailCliente()
        {
            EnviandoCorreo = true;
            StateHasChanged();

            await ServiceEmail.SendCertificateInquiryAsync(estudiante.DNI, estudiante.Nombres + " " + estudiante.Apellidos, MensajeEmail, ContactoUsuario);
            
            EnviandoCorreo = false;
            StateHasChanged();

            MensajeEmail = "";
            ContactoUsuario = "";
            MostrarCorreo = false;
            StateHasChanged();
        }
        private async Task DescargarCertificado(string archivo)
        {
            Console.WriteLine("PASE COMPA" + archivo);
            Stream stream = await S3.DownloadCertificadosAsync(archivo, estudiante.DNI);
            if (stream != null)
            {
                var fileBytes = ((MemoryStream)stream).ToArray();
                var base64 = Convert.ToBase64String(fileBytes);

                // Llamar a JS para descargar
                string fileName = Path.GetFileName(archivo);
                await JSRuntime.InvokeVoidAsync("descargarArchivo", base64, fileName);
            }
        }
    }
}
