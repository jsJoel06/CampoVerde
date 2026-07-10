using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace CampoVerde.Models
{
    public class Ingreso
    {
        [Key]
        public int IdIngreso { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        public string Concepto { get; set; }

        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Notas { get; set; }

        // --- Lo que faltaba para la relación ---

        public int? IdAnimal { get; set; }

        [ForeignKey("IdAnimal")]
        public virtual Animal Animal { get; set; }
    }
}