using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;

namespace UNTCCSS.Components.Modal
{
    public partial class EditarEstudiante
    {
        [Parameter] public Estudiante Input { get; set; } = new();
        [Inject] IEstudianteRepositorio Estudiante { get; set; }
        private async Task UpdateEstudiante()
        {
            bool response = await Estudiante.UpdateEstudiante(Input);
            if(response)
            {
                await BlazoredModal.CloseAsync(ModalResult.Ok(Input));
            }
            else
            {
                Input.Id = 0;
                await BlazoredModal.CloseAsync(ModalResult.Ok(Input));
            }
        }
    }
}
