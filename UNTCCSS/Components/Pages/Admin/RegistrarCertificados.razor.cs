using Microsoft.AspNetCore.Components;
using UNTCCSS.Helper;
using UNTCCSS.Models;
using UNTCCSS.Repositorios.IRepositorios;
using UNTCCSS.Servicios.JCASoftware;
using UNTCCSS.Servicios.JCASoftware.JCAReponses;

namespace UNTCCSS.Components.Pages.Admin
{
    public partial class RegistrarCertificados
    {
        [Inject] IJCARepositorio JCARepositorio { get; set; }
        [Inject] IEstudianteRepositorio EstudianteRepositorio { get; set; }
        [Inject] IUsersRepositorio UsersRepositorio { get; set; }
        [Inject] ICursoRepositorio CursoRepositorio { get; set; }
        [Inject] IResolucionRepositorio ResolucionRepositorio { get; set; }
        [Inject] ICertificadoRepositorio certificadoRepositorio { get; set; }
        [Inject] NavigationManager Navegacion {  get; set; }

        private ResponseConsultaDni Persona { get; set; } = new();
        private Estudiante? estudianteExistente = null;
        private bool dniBuscado = false;
        private Estudiante NuevoEstudiante = new();

        private List<Curso> Cursos = new();
        private List<Curso> CursosFiltrados = new();

        private Certificado NuevoCertificado = new();

        private List<Resolucion> Resoluciones = new();

        protected override async Task OnInitializedAsync()
        {
            if (CursoRepositorio != null)
            {
                Cursos = await CursoRepositorio.ListaCursos();
            }
            if (ResolucionRepositorio != null)
            {
                Resoluciones = await ResolucionRepositorio.Resoluciones();
            }
        }
        private async Task BuscarPersona()
        {
            if (string.IsNullOrWhiteSpace(NuevoEstudiante.DNI)) return;

            if (JCARepositorio != null && EstudianteRepositorio != null)
            {
                // Buscar en la BD
                estudianteExistente = await EstudianteRepositorio.BuscarEstudiante(NuevoEstudiante.DNI);
                dniBuscado = true;

                if (estudianteExistente == null)
                {
                    // Buscar en la API si no existe en la BD
                    Persona = await JCARepositorio.ObtenerPersona(NuevoEstudiante.DNI);
                    if (Persona.Success)
                    {
                        NuevoEstudiante.Nombres = TextFormater.ToTitleCase(Persona.Nombres);
                        NuevoEstudiante.Apellidos = TextFormater.ToTitleCase($"{Persona.ApellidoPaterno} {Persona.ApellidoMaterno}");
                    }
                }
                else
                {
                    NuevoEstudiante = estudianteExistente;
                    NuevoCertificado.AlumnoId = estudianteExistente.Id;
                }
            }
        }
        private async Task RegistrarEstudiante()
        {
            if(EstudianteRepositorio != null && UsersRepositorio != null)
            {
                var user = await UsersRepositorio.GetUserWithProfileAsync();
                if(user != null)
                {
                    NuevoEstudiante.UserId = user.Id;
                    NuevoEstudiante.AtCreated = DateTime.Now;
                    var response = await EstudianteRepositorio.RegistrarEstudiante(NuevoEstudiante);
                    if (response != null)
                    {
                        estudianteExistente = response;
                        NuevoCertificado.AlumnoId = response.Id;
                        return;
                    }
                }               
            }
            LimpiarFormulario();
        }
        private void LimpiarFormulario()
        {
            Persona.Dni = "";
            estudianteExistente = null;
            NuevoEstudiante = new();
            dniBuscado = false;
        }
        private void ActualizarCursoId(ChangeEventArgs e)
        {
            NuevoCertificado.Nombre = e.Value?.ToString() ?? "";
            if (NuevoCertificado.Nombre.Length >= 2)
            {
                FiltrarCursos(NuevoCertificado.Nombre);
            }
            else
            {
                CursosFiltrados.Clear();
            }

            var cursoSeleccionado = CursosFiltrados?.FirstOrDefault(c => c.Nombre == NuevoCertificado.Nombre);
            if (cursoSeleccionado != null)
            {
                NuevoCertificado.CursoId = cursoSeleccionado.Id;
            }
            else
            {

                NuevoCertificado.CursoId = 0;
            }
        }
        private void FiltrarCursos(string texto)
        {
            if (CursoRepositorio != null)
            {
                CursosFiltrados = Cursos
                .Where(c => c.Nombre.ToLower().Contains(texto.Trim().ToLower()))
                .ToList();
            }
        }
        private void GenerarCodigo()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            NuevoCertificado.Codigo = new string(Enumerable.Range(0, 8)
                                       .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }


        private bool IsProcessing = false;
        private bool IsSuccess = false;
        private string ButtonText = "Guardar Certificado";
        private string ButtonIcon = "fas fa-save";
        private async Task RegistrarCertificado()
        {
            if(certificadoRepositorio != null && UsersRepositorio != null)
            {
                IsProcessing = true;
                IsSuccess = false;
                ButtonText = "Guardando...";
                ButtonIcon = "fas fa-spinner fa-spin";

                var user = await UsersRepositorio.GetUserWithProfileAsync();
                if (user != null)
                {
                    var validacioncurso = await CursoRepositorio.ExisteCurso(NuevoCertificado.CursoId);
                    if(!validacioncurso)
                    {
                        NuevoCertificado.CursoId = null;
                    }
                    NuevoCertificado.UserId = user.Id;
                    var response = await certificadoRepositorio.RegistrarCertificado(NuevoCertificado);
                    if(response)
                    {
                        //implemnetar un tiempo de espera
                        IsProcessing = false;
                        IsSuccess = true;
                        ButtonText = "Guardado";
                        ButtonIcon = "fas fa-check-circle";
                        await Task.Delay(2000);

                        Navegacion.NavigateTo("/admin/certificados", true);
                    }
                }

                IsProcessing = true;
                IsSuccess = false;
                ButtonText = "Guardando...";
                ButtonIcon = "fas fa-spinner fa-spin";
            }
        }
    }
}
