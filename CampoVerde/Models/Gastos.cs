using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Gasto
    {
        [Key]
        public int IdGasto { get; set; }

        // Relación opcional (si el gasto fue para un animal en específico, ej: una cirugía)
        public int? IdAnimal { get; set; }

        public decimal Monto { get; set; }
        public string Concepto { get; set; }
        public DateTime Fecha { get; set; }

        public CategoriaGasto Categoria { get; set; }

        public string Notas { get; set; }

        // En tu modelo Gasto.cs
        public virtual Animal Animal { get; set; } 
    }
}