using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }


        [Required]
        public string nombre { get; set; }


        [Required]
        [EmailAddress]
        public string email { get; set; }


        [Required]
        public string password { get; set; }


        public bool estadoActivo { get; set; } = true;


        public DateTime UltimoAcceso { get; set; }


        public string telefono { get; set; }


        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;


        public EstadoRol Rol { get; set; }


        public string? FotoPerfil { get; set; }




        // RELACIÓN CON CLIENTE

        // Puede ser null porque SUPER_ADMINISTRADOR
        // no pertenece a ninguna finca

        public int? ClienteId { get; set; }


        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }


       
    }
}