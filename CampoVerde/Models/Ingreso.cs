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
        public DateTime Fecha { get; set; } = DateTime.UtcNow;


        public string Notas { get; set; }




        // RELACIÓN CON CLIENTE (FINCA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }






        // RELACIÓN OPCIONAL CON ANIMAL

        public int? IdAnimal { get; set; }


        [ForeignKey("IdAnimal")]
        public virtual Animal Animal { get; set; }

    }
}