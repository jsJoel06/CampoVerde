using System.ComponentModel.DataAnnotations;


namespace CampoVerde.Models
{
    public class Vacuna
    {
        [Key]
        public int IdVacuna { get; set; }

        public int IdAnimal { get; set; }

        public string nombreVacuna { get; set; }

        public int frecuenciaMeses { get; set; }

        
        public DateTime fechaAplicacion { get; set; }

        // CORRECCIÓN: Usar { get; set; }
        public DateTime fechaProximaAplicacion { get; set; }

        public string observaciones { get; set; }
    }
}