using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Produccion
    {
        [Key]
        public int IdProduccion { get; set; }



        // RELACIÓN CON CLIENTE (FINCA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }



        // ANIMAL

        public int IdAnimal { get; set; }


        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }




        // PRODUCCIÓN

        public DateTime fechaProduccion { get; set; } = DateTime.UtcNow;


        public double cantidadLeche { get; set; }


        public string? observaciones { get; set; }


        public EstadoTurno Turno { get; set; }

    }
}