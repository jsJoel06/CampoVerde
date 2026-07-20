using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class ResumenFinanciero
    {
        [Key]
        public int IdResumen { get; set; }


        // RELACIÓN CON CLIENTE (FINCA)
        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }


        // PERIODO DEL RESUMEN

        [Required]
        public int Mes { get; set; }


        [Required]
        public int Año { get; set; }



        // DATOS FINANCIEROS

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalIngresos { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalGastos { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Ganancia { get; set; }



        // FECHA DE CREACIÓN

        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    }
}