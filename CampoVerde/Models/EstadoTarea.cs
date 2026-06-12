using System.ComponentModel.DataAnnotations;

namespace CampoVerde.Models
{
    public enum EstadoTarea
    {
        [Display(Name = "Pendiente")]
        Pendiente = 0,

        [Display(Name = "En Progreso")]
        EnProgreso = 1,

        [Display(Name = "Completada")]
        Completada = 2,

        [Display(Name = "Cancelada")]
        Cancelada = 3
    }
}
