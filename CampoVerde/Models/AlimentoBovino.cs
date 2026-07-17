using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampoVerde.Models
{
    // Definición de Enums para mayor control y consistencia
    public enum CategoriaAlimento
    {
        Forraje,
        Concentrado,
        Suplemento,
        Minerales
    }

    public enum UnidadMedida
    {
        Sacos,
        Pacas,
        Quintales,
        Litros,
        Toneladas
    }

    public class AlimentoBovino
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del insumo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public CategoriaAlimento Categoria { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, 999999, ErrorMessage = "La cantidad debe ser un valor positivo.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadDisponible { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public UnidadMedida Unidad { get; set; }

        [Required]
        [Range(0, 5000, ErrorMessage = "El nivel de alerta debe ser un valor positivo.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NivelAlerta { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public int? ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }
    }
}