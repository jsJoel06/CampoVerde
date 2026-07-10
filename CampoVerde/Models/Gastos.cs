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

        // Mapeo explícito de la llave foránea
        public int? IdAnimal { get; set; }

        [ForeignKey("IdAnimal")] 
        public virtual Animal Animal { get; set; }
    }
}