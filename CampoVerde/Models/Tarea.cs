using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required] // Obliga a que siempre haya un animal asociado
        public int IdAnimal { get; set; }

        [Required]
        [StringLength(200)] 
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaVencimiento { get; set; }

        public bool Completada { get; set; } = false;

        public PrioridadTarea Prioridad { get; set; } = PrioridadTarea.Media;

        [StringLength(100)]
        public string Encargado { get; set; }

        public string Notas { get; set; }

        // Relación de navegación (Opcional, pero muy útil para EF Core)
        [ForeignKey("IdAnimal")]
        public virtual Animal Animal { get; set; }

        public EstadoTarea estado { get; set; } = EstadoTarea.Pendiente;
    }
}