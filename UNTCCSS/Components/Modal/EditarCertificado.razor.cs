using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Modal
{
    public partial class EditarCertificado : ComponentBase
    {
        [Inject] IResolucionRepositorio resolucion {  get; set; }
        [Inject] ICertificadoRepositorio Certificado {  get; set; }

        private List<Resolucion> Resoluciones = new();
        public Certificado? Input = new();
        public EditarCertificado()
        {
            
        }
        protected override async Task OnInitializedAsync()
        {
            Resoluciones = await resolucion.Resoluciones();
            if(CertificadoId > 0)
            {
                Input = await Certificado.OptenerCertificado(CertificadoId);
            }
            else
            {
                await BlazoredModal.CancelAsync(true);
            }
        }


        private bool IsProcessing = false;
        private bool IsSuccess = false;
        private string ButtonText = "Guardar Cambios";
        private string ButtonIcon = "fas fa-save";
        private async Task UpdateCert()
        {
            if(Input != null)
            {
                bool update = await Certificado.ActualizarCertificado(Input);
                if(update)
                {
                    await BlazoredModal.CloseAsync(ModalResult.Ok(Input));
                }

            }
        }
    }
}
