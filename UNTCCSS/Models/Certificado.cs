using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UNTCCSS.Data;

namespace UNTCCSS.Models
{
    public class Certificado
    {
        [Key]
        public int Id { get; set; }

        [Required][MaxLength(200)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoDocumento { get; set; }

        public int IdResolucion { get; set; }
        [ForeignKey("IdResolucion")]
        public Resolucion Resolucion { get; set; }

        public int PromedioFinal {  get; set; }

        [MaxLength(50)]
        public string Codigo { get; set; }

        public bool Status {  get; set; }

        public bool IsDelete { get; set; }

        [MaxLength(50)]
        public string Registro { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser Usuario { get; set; }

        public int? CursoId { get; set; }
        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }

        public DateTime FechaInicio { get; set; } 
        public DateTime FechaTermino { get; set; } 
        public DateTime FechaEmision { get; set; } 
        public DateTime RegCreated { get; set; } 
        public DateTime RegUpdated { get; set; }

        public int AlumnoId { get; set; }
        [ForeignKey("AlumnoId")]
        public Estudiante Alumno {  get; set; }

        public string Archivo {  get; set; }
    }
}
