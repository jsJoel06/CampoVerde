using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Parto
    {
        [Key]
        public int IdParto { get; set; }



        // RELACIÓN CON CLIENTE (FINCA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }




        // Madre

        [Required]
        public int IdAnimal { get; set; }


        [ForeignKey("IdAnimal")]
        public virtual Animal? Animal { get; set; }





        // Fecha del parto

        [Required]
        public DateTime FechaParto { get; set; } = DateTime.UtcNow;





        // Cría

        [Required]
        public string NombreCria { get; set; } = string.Empty;


        [Required]
        public string CodigoCria { get; set; } = string.Empty;


        [Required]
        public string SexoCria { get; set; } = string.Empty;


        [Required]
        public double PesoCria { get; set; }


        [Required]
        public EstadoCria EstadoCria { get; set; }



        public string? Observaciones { get; set; }

    }
}