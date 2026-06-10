namespace CampoVerde.Models
{
    public enum TipoTransaccion { Compra, Venta }

    public class TransaccionAnimal
    {
        public int IdTransaccion { get; set; }
        public int IdAnimal { get; set; }
        public TipoTransaccion Tipo { get; set; } // Compra o Venta
        public decimal Monto { get; set; }
        public string Tercero { get; set; } // ¿A quién le compraste o a quién vendiste?
        public DateTime Fecha { get; set; }
        public string Notas { get; set; }
    }
}
