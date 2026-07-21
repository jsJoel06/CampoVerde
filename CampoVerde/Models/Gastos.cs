using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Gasto
    {
        [Key]
        public int IdGasto { get; set; }


        public decimal Monto { get; set; }


        public string Concepto { get; set; }


        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }



        public CategoriaGasto Categoria { get; set; }


        public string Notas { get; set; }




        // RELACIÓN CON CLIENTE (FINCA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }





        // RELACIÓN OPCIONAL CON ANIMAL

        public int? IdAnimal { get; set; }


        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }

    }
}