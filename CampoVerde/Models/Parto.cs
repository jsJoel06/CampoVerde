using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Parto
    {

        [Key]
        public int idParto { get; set; }
        public int idAnimal { get; set; }

        public DateTime FechaParto { get; set; }
        public string SexoCria { get; set; }
        public string PesoCria { get; set; }

        public EstadoCria EstadoCria { get; set; }

        public string Observaciones { get; set; }
    }
}
