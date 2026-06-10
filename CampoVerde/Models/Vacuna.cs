using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Vacuna
    {


        [Key]
        public int idVacuna { get; set; }
        public int idAnimal { get; set; }
        public string nombreVacuna { get; set; }
        public DateTime fechaAplicacion;
        public DateTime fechaProximaAplicacion;
        public string observaciones { get; set; }
    }
}
