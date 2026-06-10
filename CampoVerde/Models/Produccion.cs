using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Produccion
    {
        [Key]
        public int idProduccion { get; set; }

        public int idAnimal { get; set; }
        public DateTime fechaProduccion;
        public double cantidadLeche { get; set; }
        public string observaciones { get; set; }

        public EstadoTurno Turno { get; set; }
    }
}
