using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Animal
    {

        [Key]
        public int idAnimal { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }  
        public DateTime? fechaNacimiento { get; set; }

        public string raza { get; set; }


        public double pesoActual { get; set; }
        public string lote { get; set; }


        public string? imagen { get; set; }

        public EstadoAnimal Estado { get; set; } = EstadoAnimal.ACTIVO;

    }
}
