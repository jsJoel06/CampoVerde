using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Parto
    {

        [Key]
        public int IdParto { get; set; }
        public int IdAnimal { get; set; }

        public DateTime FechaParto { get; set; }
        public string SexoCria { get; set; }
        public string PesoCria { get; set; }

        public EstadoCria EstadoCria { get; set; }

        public string Observaciones { get; set; }
    }
}
