using System;
using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public bool Leida { get; set; } = false;
    }
}