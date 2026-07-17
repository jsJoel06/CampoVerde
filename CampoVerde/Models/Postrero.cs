using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    public class Potrero
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string Nombre { get; set; }




        // RELACIÓN CON CLIENTE (FINCA)

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }



        // Fotos

        // Almacenamos:
        // "foto1.jpg;foto2.jpg"

        public string? RutasFotos { get; set; }



        // No se guarda en BD

        [NotMapped]
        public List<IFormFile>? ArchivosFotos { get; set; }

    }
}