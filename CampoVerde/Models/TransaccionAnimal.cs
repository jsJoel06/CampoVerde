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


        public int? ClienteId { get; set; }

        public Cliente? Cliente { get; set; }



        public int? IdAnimal { get; set; }

        public Animal? Animal { get; set; }



        public TipoTransaccion Tipo { get; set; }



        public decimal Monto { get; set; }



        public string Tercero { get; set; }



        // Datos solo para compra

        public string? NombreAnimal { get; set; }

        public string? Raza { get; set; }

        public decimal? Peso { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public string? Proveedor { get; set; }



        public DateTime Fecha { get; set; }
            = DateTime.UtcNow;


        public string? Notas { get; set; }

    }
}