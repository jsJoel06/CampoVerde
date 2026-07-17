using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public enum TipoTransaccion
    {
        Compra,
        Venta
    }


    public class TransaccionAnimal
    {
        [Key]
        public int IdTransaccion { get; set; }



        // RELACIÓN CON CLIENTE (FINCA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }




        // RELACIÓN CON ANIMAL

        public int IdAnimal { get; set; }


        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }




        public TipoTransaccion Tipo { get; set; }




        public decimal Monto { get; set; }



        // Persona o empresa con quien se realizó la operación

        public string Tercero { get; set; }



        public DateTime Fecha { get; set; } = DateTime.UtcNow;



        public string? Notas { get; set; }

    }
}