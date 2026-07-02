using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Potrero
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Almacenamos los nombres/rutas de los archivos: "foto1.jpg;foto2.jpg;..."
        public string? RutasFotos { get; set; }

        // Propiedad auxiliar para el formulario (no se guarda en BD)
        [NotMapped]
        public List<IFormFile>? ArchivosFotos { get; set; }
    }
}