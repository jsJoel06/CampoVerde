using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }



        // Datos de la finca o empresa cliente

        [Required]
        public string Nombre { get; set; }


        [Required]
        public string Telefono { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }


        public string Direccion { get; set; }



        // Información del servicio CampoVerde

        [Required]
        public string NombreEmpresa { get; set; }


        [Required]
        public string Plan { get; set; }
        // Básico, Profesional, Premium


        public DateTime FechaContratacion { get; set; } = DateTime.UtcNow;


        public DateTime? FechaVencimiento { get; set; }


        public bool Activo { get; set; } = true;



        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;




        // ==========================
        // RELACIONES
        // ==========================


        // Usuarios de esta empresa

        public ICollection<Usuario> Usuarios { get; set; }
            = new List<Usuario>();



        // Animales de esta finca

        public ICollection<Animal> Animales { get; set; }
            = new List<Animal>();



        // Producción de leche

        public ICollection<Produccion> Producciones { get; set; }
            = new List<Produccion>();



        // Vacunas aplicadas

        public ICollection<Vacuna> Vacunas { get; set; }
            = new List<Vacuna>();



        // Tareas pendientes

        public ICollection<Tarea> Tareas { get; set; }
            = new List<Tarea>();

        // Partos registrados
        public ICollection<Parto> Partos { get; set; }
            = new List<Parto>();

        // Gastos
        public ICollection<Gasto> Gastos { get; set; }
            = new List<Gasto>();

        // Ingresos
        public ICollection<Ingreso> Ingresos { get; set; }
            = new List<Ingreso>();

        // Potreros
        public ICollection<Potrero> Potreros { get; set; }
            = new List<Potrero>();



        // Compras y ventas de animales
        public ICollection<TransaccionAnimal> TransaccionesAnimales { get; set; }
            = new List<TransaccionAnimal>();

        public ICollection<Licencia> Licencias { get; set; } 
            = new List<Licencia>();

    }
}