using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Vacuna
    {
        [Key]
        public int IdVacuna { get; set; }

        [Required]
        public int IdAnimal { get; set; }

        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }

        [Required]
        public string nombreVacuna { get; set; } = string.Empty;

        [Required]
        public int frecuenciaMeses { get; set; }

        [Required]
        public DateTime fechaAplicacion { get; set; }

        public DateTime fechaProximaAplicacion { get; set; }

        public string? observaciones { get; set; }
    }
}