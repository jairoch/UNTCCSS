using UNTCCSS.Components.Modal;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Pages
{
    public partial class Consultas : ComponentBase
    {
        [Inject] IEstudianteRepositorio EstudianteRepositorio { get; set; }
        [Inject] IAutenticacionRepositorio AutenticacionRepositorio { get; set; }
        [Inject] NavigationManager Navigation { get; set; }
        [CascadingParameter] public IModalService Modal { get; set; } = default!;
        private string DNI = string.Empty;

        private void MostrarModal()
        {
            var parameters = new ModalParameters();
            parameters.Add("Title", "Consulta Detallada");

            var options = new ModalOptions() { Class = "modal-config" };

            Modal.Show<MasInformation>("Información", parameters, options);
        }
        private bool Cargando = false;
        private bool mostrarResultado = false;
        private string mensajeResultado = string.Empty;
        private async void Validar()
        {
            mostrarResultado = false;
            Cargando = true;
            if (EstudianteRepositorio != null)
            {
                var response = await EstudianteRepositorio.ValidarCursosEstudiante(DNI);
                if(response)
                {
                    int IdEstudiante = await EstudianteRepositorio.ObtenerIdEstudiante(DNI);
                    string token = AutenticacionRepositorio.GenerarTokenConsultaCert(IdEstudiante);
                    Navigation.NavigateTo($"/consulta-exitosa/{token}");
                }
                else
                {
                    mostrarResultado=true;
                    mensajeResultado = "Lo sentimos, no encontramos tu registro";

                }
            }
            Cargando = false;
            StateHasChanged();
        }
    }
}
