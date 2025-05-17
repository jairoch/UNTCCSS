using UNTCCSS.Components.Modal;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace UNTCCSS.Components.Modal.ModalServicios
{
    public class ConfirmacionServicio
    {
        private readonly IModalService Modal;
        public ConfirmacionServicio(IModalService modal)
        {
            Modal = modal;
        }
        public async Task<bool> MostrarConfirmacion(string titulo, string mensaje, string tema = "error")
        {
            var parameters = new ModalParameters();
            parameters.Add("Title", titulo);
            parameters.Add("Mensaje", mensaje);
            parameters.Add("Tema", tema);

            var options = new ModalOptions { Class = "modal-config" };

            var result = await Modal.Show<ModalConfirmacion>("Confirmación", parameters, options).Result;

            return !result.Cancelled && result.Data is bool aceptado && aceptado;
        }
    }
}
