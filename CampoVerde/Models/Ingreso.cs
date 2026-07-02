using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Ingreso
    {
        [Key]
        public int IdIngreso { get; set; }

        public int? IdAnimal { get; set; }

        public decimal Monto { get; set; }

        public string Concepto { get; set; }

        public DateTime Fecha { get; set; }

        public string Notas { get; set; }
    }
}
