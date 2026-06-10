using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Usuario
    {
        [Key]
        public int idUsuario { get; set; }
        public string nombre {  get; set; }
        public string email { get; set; }
        public string password;
        public bool estadoActivo = true;
        public DateTime UltimoAcceso;
        public string telefono { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public EstadoRol Rol { get; set; } 
    }
}
