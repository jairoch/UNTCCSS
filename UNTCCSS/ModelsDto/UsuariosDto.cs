using UNTCCSS.Data;
using UNTCCSS.Models;

namespace UNTCCSS.ModelsDto
{
    public class UsuariosDto
    {
        public UsuariosDto()
        {
            Roles = new List<ApplicationRole>();
            Permisos = new List<Permisos>();
        }
        public string Id { get; set; }
        public string Dni { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; } = "Sin correo";
        public bool Blokeado { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        public List<Permisos> Permisos { get; set; }
    }
}
