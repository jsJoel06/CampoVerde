using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Produccion
    {
        [Key]
        public int IdProduccion { get; set; }

        public int IdAnimal { get; set; }

        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }

        public DateTime fechaProduccion { get; set; }

        public double cantidadLeche { get; set; }

        public string? observaciones { get; set; }

        public EstadoTurno Turno { get; set; }
    }
}
