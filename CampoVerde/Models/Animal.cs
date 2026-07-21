using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Animal
    {

        [Key]
        public int IdAnimal { get; set; }


        public string codigo { get; set; }


        public string nombre { get; set; }


        public DateTime? fechaNacimiento { get; set; }


        public string raza { get; set; }


        public double pesoActual { get; set; }


        public string lote { get; set; }


        public string? imagen { get; set; }


        public EstadoAnimal Estado { get; set; } = EstadoAnimal.ACTIVO;

        public DateTime? FechaEmbarazo { get; set; }



        // RELACIÓN CON CLIENTE (FINCA/EMPRESA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

    }
}