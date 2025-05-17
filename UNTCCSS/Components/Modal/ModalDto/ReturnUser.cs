using UNTCCSS.ModelsDto;

namespace UNTCCSS.Components.Modal.ModalDto
{
    public class ReturnUser
    {
        public bool Success {  get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Mensaje {  get; set; } = string.Empty ;
        public NuevoUsuarioDto NuevoUsuario { get; set; }
    }
}
