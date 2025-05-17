namespace UNTCCSS.ModelsDto
{
    public class RolResumenDto
    {
        public string NombreRol { get; set; } = string.Empty;
        public int CantidadUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosSuspendidos { get; set; }
        public List<string> Permisos { get; set; } = new();
    }
}
