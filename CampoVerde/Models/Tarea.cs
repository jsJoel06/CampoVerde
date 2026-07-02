using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CampoVerde.Models
{
    public enum PrioridadTarea
    {
        Baja,
        Media,
        Alta,
        Urgente
    }

    public class Tarea
    {
        [Key] // Marca este campo como la Llave Primaria
        public int IdTarea { get; set; }

        [Required] 
        public int IdAnimal { get; set; }

        [Required]
        [StringLength(200)] 
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime FechaVencimiento { get; set; }

        public bool Completada { get; set; } = false;

        public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Media;

        [StringLength(100)]
        public string? Encargado { get; set; }

        public string? Notas { get; set; }


        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }

        public EstadoTarea estado { get; set; } = EstadoTarea.Pendiente;
    }
}