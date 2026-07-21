using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Licencia
    {
        [Key]
        public int IdLicencia { get; set; }


        [Required]
        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; }


        [Required]
        public string Plan { get; set; }


        [Required]
        public DateTime FechaInicio { get; set; }


        [Required]
        public DateTime FechaVencimiento { get; set; }


        public bool Activa { get; set; } = true;


        public string Estado
        {
            get
            {
                if (FechaVencimiento < DateTime.UtcNow)
                    return "VENCIDA";

                return "ACTIVA";
            }
        }
    }
}