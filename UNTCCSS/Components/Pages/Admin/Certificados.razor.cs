using UNTCCSS.Components.Modal;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.VisualBasic.FileIO;
using Org.BouncyCastle.Ocsp;
using System.Net;
using UNTCCSS.Components.Modal.ModalServicios;
using UNTCCSS.Data;
using UNTCCSS.Models;
using UNTCCSS.ModelsDto;
using UNTCCSS.Repositorios;
using UNTCCSS.Repositorios.IRepositorios;
using UNTCCSS.Servicios.AWS;

namespace UNTCCSS.Components.Pages.Admin
{
    public partial class Certificados : ComponentBase
    {
        [CascadingParameter] public ApplicationUser User { get; set; }
        [Inject] private ConfirmacionServicio ConfirmacionService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IUsersRepositorio usersRepositorio { get; set; }
        [Inject] ICertificadoRepositorio certificadoRepositorio { get; set; }
        [Inject] IUsersRepositorio UsersRepositorio { get; set; }
        [Inject] IEstudianteRepositorio estudianteRepositorio { get; set; }
        [Inject] IAutenticacionRepositorio AuthService { get; set; }
        [Inject] IS3Service S3Service { get; set; }

        private List<Estudiante> ListEstudiantes = new();
        private List<Certificado> ListCertificados { get; set; } = new();
        private ResponseRolesPermisos RolesPermisos { get; set; } = new();
        private string searchText;

        private int Pagina;
        private int TotalPaginas;
        private AuthData authData = new();

        public Certificados()
        {
            
        }
        protected override async Task OnInitializedAsync()
        {
            authData = await AuthService.ObtenerDatosAutenticacionAsync();
            Pagina = 1;
            await FiltrarTabla(FiltroTipo);
            RefrescarList();
        }
        private void RefrescarList()
        {
            if (ListCertificados.Count > 0)
            {
                ListEstudiantes = ListCertificados
                    .Select(c => c.Alumno)
                    .Distinct() // Eliminar duplicados
                    .ToList();
            }
        }
        private void RegistrarNuevo()
        {
             NavigationManager.NavigateTo("/admin/certificados/registrarCertificados");
        }
        private async Task EditarCertificado(int CertificadoId)
        {
            var parameters = new ModalParameters();
            parameters.Add("Title", "Editar Certificado");
            parameters.Add("CertificadoId", CertificadoId);

            var options = new ModalOptions { Class = "modal-config" };
            var response = await Modal.Show<EditarCertificado>("Editar Certificado", parameters, options).Result;
            if (!response.Cancelled && response.Data is Certificado returnCertificado)
            {
                if (returnCertificado != null)
                {
                    var estudiante = ListEstudiantes.FirstOrDefault(e => e.Certificados.Any(c => c.Id == CertificadoId));
                    if (estudiante != null)
                    {
                        int certIndex = estudiante.Certificados.FindIndex(c => c.Id == CertificadoId);

                        if (certIndex != -1)
                        {
                            estudiante.Certificados[certIndex] = returnCertificado;
                        }
                    }

                    Message = "Success: Registro actualizado correctamente";
                    SetMessage(Message, 4);
                }
                else
                {
                    Message = "Error: Lo Sentimos a ocurrido un error al actualizar el registro ";
                    SetMessage(Message, 4);
                }
            }
        }
        private string Message;
        private async void SetMessage(string mensaje, int segundos)
        {
            Message = mensaje;
            StateHasChanged();
            await Task.Delay(segundos * 1000);
            Message = null;
            StateHasChanged();
        }
        private async Task PausarVerificacion(int CertificadoId)
        {
            bool response = await certificadoRepositorio.PausarVerificacion(CertificadoId);
            if (response)
            {
                var estudiante = ListEstudiantes.FirstOrDefault(e => e.Certificados.Any(c => c.Id == CertificadoId));
                if (estudiante != null)
                {
                    // 📌 Buscar el certificado en la lista del estudiante y actualizar su Status
                    var certificado = estudiante.Certificados.FirstOrDefault(c => c.Id == CertificadoId);
                    if (certificado != null)
                    {
                        certificado.Status = false;
                    }
                }

                Message = "Warning: ✅ La verificación ha sido pausada. El certificado no estará disponible en la consulta del participante hasta que se reactive.";
                SetMessage(Message, 4);
            }
        }
        private async Task ReanudarVerificacion(int CertificadoId)
        {
            bool response = await certificadoRepositorio.ReanudarVerificacion(CertificadoId);
            if (response)
            {
                var estudiante = ListEstudiantes.FirstOrDefault(e => e.Certificados.Any(c => c.Id == CertificadoId));
                if (estudiante != null)
                {
                    // 📌 Buscar el certificado en la lista del estudiante y actualizar su Status
                    var certificado = estudiante.Certificados.FirstOrDefault(c => c.Id == CertificadoId);
                    if (certificado != null)
                    {
                        certificado.Status = true;
                    }
                }

                Message = "✅ La verificación ha sido reanudada. El certificado ahora estará disponible en la consulta del participante.";
                SetMessage(Message, 4);
            }
        }
        private async Task EliminarEstudiante(int EstudianteId)
        {

            string mensaje = "⚠️ Esta acción es irreversible. ¿Estás seguro de que deseas eliminar el estudiante ten en cuenta" +
                             " que se eliminaran tambien sus certificados asociados?";

            bool aceptado = await ConfirmacionService.MostrarConfirmacion("Eliminar estudiante", mensaje, "warning");
            if (aceptado)
            {
                bool response = await estudianteRepositorio.Delete(EstudianteId);
                if (response)
                {
                    var estudiante = ListEstudiantes.FirstOrDefault(e => e.Id == EstudianteId);

                    if (estudiante != null)
                    {
                        estudiante.Certificados.Clear();
                        ListEstudiantes.Remove(estudiante);
                    }
                    // 📌 Mensaje claro para el usuario
                    Message = "🗑️ El estudiante ha sido eliminado junto con todos sus certificados asociados.";

                    SetMessage(Message, 4);
                    StateHasChanged();
                }
            }
        }
        private async Task EditarEstudiante(int EstudianteId)
        {
            var Input = ListEstudiantes.FirstOrDefault(e => e.Id == EstudianteId);
            var parameters = new ModalParameters();
            parameters.Add("Title", "Editar Estudiante");
            parameters.Add("Input", Input);

            var options = new ModalOptions { Class = "modal-config" };
            var response = await Modal.Show<EditarEstudiante>("Editar Estudiante", parameters, options).Result;
            if (!response.Cancelled && response.Data is Estudiante returnEstudiante)
            {
                if(returnEstudiante.Id > 0)
                {
                    var index = ListEstudiantes.FindIndex(e => e.Id == returnEstudiante.Id);
                    ListEstudiantes[index] = returnEstudiante;
                    Message = "Success: Registro actualizado correctamente.";
                    SetMessage(Message, 4);
                }
                else
                {
                    Message = "Error: Lo sentimos a ocurrido un error.";
                    SetMessage(Message, 4);
                }
            }
        }
        private async Task Delete(int CertificadoId)
        {
            string mensaje = "⚠️ *Acción irreversible:* ¿Estás seguro de que deseas eliminar este certificado? \n\n" +
                 "📌 *Importante:* Una vez eliminado, el certificado ya no estará disponible en la página de consulta del participante, " +
                 "y no podrá ser recuperado. Asegúrese de que esta acción es necesaria antes de continuar.";


            bool aceptado = await ConfirmacionService.MostrarConfirmacion("Eliminar Certificado", mensaje, "warning");
            if (aceptado)
            {
                bool response = await certificadoRepositorio.Delete(CertificadoId);
                if (response)
                {
                    var estudiante = ListEstudiantes.FirstOrDefault(e => e.Certificados.Any(c => c.Id == CertificadoId));

                    if (estudiante != null)
                    {
                        // 📌 Eliminar el certificado de la lista del estudiante
                        estudiante.Certificados.RemoveAll(c => c.Id == CertificadoId);

                        // 📌 Si el estudiante ya no tiene más certificados, eliminarlo de la lista de estudiantes
                        if (!estudiante.Certificados.Any())
                        {
                            ListEstudiantes.Remove(estudiante);
                        }
                    }

                    Message = "🗑️ Certificado eliminado correctamente.";
                    SetMessage(Message, 4);
                }
            }
        }

        string FiltroTipo = "Hoy";
        private DateTime FechaInicio;
        private DateTime FechaFin;
        private async Task FiltrarTabla(string filtro)
        {
            FiltroTipo = filtro;

            var hoy = DateTime.Now;

            switch (filtro)
            {
                case "Hoy":
                    FechaInicio = hoy;
                    FechaFin = hoy;
                    break;

                case "Ayer":
                    FechaInicio = hoy.AddDays(-1);
                    FechaFin = hoy.AddDays(-1);
                    break;

                case "Últimos 7 días":
                    FechaInicio = hoy.AddDays(-7);
                    FechaFin = hoy;
                    break;

                case "Últimos 30 días":
                    FechaInicio = hoy.AddDays(-30);
                    FechaFin = hoy;
                    break;

                case "Este Mes":
                    FechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
                    FechaFin = FechaInicio.AddMonths(1).AddDays(-1);
                    break;

                case "Año Actual":
                    FechaInicio = new DateTime(hoy.Year, 1, 1);
                    FechaFin = new DateTime(hoy.Year, 12, 31);
                    break;

                case "Año Anterior":
                    FechaInicio = new DateTime(hoy.Year - 1, 1, 1);
                    FechaFin = new DateTime(hoy.Year - 1, 12, 31);
                    break;

                default:
                    FechaInicio = hoy;
                    FechaFin = hoy;
                    break;
            }

            if (authData.Roles.Contains("Admin") || authData.Permisos.Contains("VerCertificadosAdmin"))
            {
                ListCertificados = await certificadoRepositorio.ListarTodos(FechaInicio, FechaFin, Pagina);
                RefrescarList();
                TotalPaginas = certificadoRepositorio.TotalPages;
            }
            else
            {
                ListCertificados = await certificadoRepositorio.ListarFiltrado(FechaInicio, FechaFin, Pagina);
                RefrescarList();
                TotalPaginas = certificadoRepositorio.TotalPages;
            }


        }
        private async Task CambiarPagina(int Pagina)
        {
            this.Pagina = Pagina;
            await FiltrarTabla(FiltroTipo);
        }
        private async Task BuscarEstudiantes()
        {
            if (authData.Roles.Contains("Admin") || authData.Permisos.Contains("VerCertificadosAdmin"))
            {
                ListCertificados = await certificadoRepositorio.FiltrarTodosPorDniAdmin(searchText);
                if(ListCertificados.Count <= 0)
                {
                    Message = "No se encontraron registros con el DNI ingresado.";
                    SetMessage(Message, 4);
                    return;
                }
                RefrescarList();
                TotalPaginas = 1;
                Pagina = 1;
            }
            else
            {
                ListCertificados = await certificadoRepositorio.FiltrarTodosPorDniEmpresa(searchText);
                if (ListCertificados.Count <= 0)
                {
                    Message = "No se encontraron registros con el DNI ingresado.";
                    SetMessage(Message, 4);
                    return;
                }
                RefrescarList();
                TotalPaginas = 1;
                Pagina = 1;
            }
        }



        private Dictionary<int, bool> CargandoCertificados = new();
        public string FileName { get; set; } = "";
        public long FileSize { get; set; }
        public string FileType { get; set; } = "";
        public DateTimeOffset LastModified { get; set; }
        public string ErrorMessage { get; set; } = "";

        const int MAX_FILESIZE = 5 * 1024 * 1024; // 5 MB
        private Stream ArchivoSeleccionado;
        private string GetInputFileId(int certId) => $"fileInput-{certId}";
        private async Task FileUploaded(InputFileChangeEventArgs e, Estudiante estudiante, Certificado certificado)
        {
            var browserFile = e.File;

            if (browserFile != null)
            {
                CargandoCertificados[certificado.Id] = true;

                FileSize = browserFile.Size;
                FileType = browserFile.ContentType;
                FileName = browserFile.Name;
                LastModified = browserFile.LastModified;

                // Validar tamaño máximo
                if (FileSize > MAX_FILESIZE)
                {
                    ErrorMessage = "Arhivo máximo permitido: 5 MB.";
                    return;
                }

                try
                {
                    ArchivoSeleccionado = browserFile.OpenReadStream(MAX_FILESIZE);
                    string fechaFormateada = DateTime.Now.ToString("yyyyMMdd");
                    string key = $"UNT/Certificados/{estudiante.DNI}/{estudiante.DNI}-{certificado.Codigo}-{fechaFormateada}.pdf";
                    string archivo = await ExisteArchivo(certificado.Registro, certificado.Codigo);
                    bool resultado;
                    if(archivo != null)
                    {
                        resultado = await S3Service.SubirArchivoAsync(ArchivoSeleccionado, estudiante.DNI, archivo);
                    }
                    else
                    {
                        resultado = await S3Service.SubirArchivoAsync(ArchivoSeleccionado, estudiante.DNI, key);
                        certificadoRepositorio.InsertarNombreArchivo(certificado.Id,key);
                    }  
                    
                    if (resultado)
                    {
                        ErrorMessage = "Archivo subido exitosamente.";
                    }
                    else
                    {
                        ErrorMessage = "Error al subir el archivo.";
                    }
                }
                catch (Exception exception)
                {
                    ErrorMessage = exception.Message;
                }
                CargandoCertificados[certificado.Id] = false;
                StateHasChanged();
            }
        }
        private async Task<string> ExisteArchivo(string Registro, string Codigo)
        {
            return await certificadoRepositorio.ComprobarArchivo(Registro, Codigo);
        }
    }
}

