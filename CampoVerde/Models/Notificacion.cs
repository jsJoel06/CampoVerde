using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public bool Leida { get; set; } = false;


        public int? ClienteId { get; set; }


        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
    }
}