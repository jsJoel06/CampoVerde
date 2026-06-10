using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Ingreso
    {
        [Key]
        public int idIngreso { get; set; }

        public int? idAnimal { get; set; }

        public decimal Monto { get; set; }

        public string Concepto { get; set; }

        public DateTime Fecha { get; set; }

        public string Notas { get; set; }
    }
}
