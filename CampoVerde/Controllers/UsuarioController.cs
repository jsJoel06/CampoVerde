using Microsoft.AspNetCore.Mvc;

namespace CampoVerde.Controllers
{
    public class UsuarioController : Controller
    {
        // Al quitar el 'int id', la ruta /Usuario/Perfi funcionará sin parámetros
        [HttpGet]
        public IActionResult Perfil()
        {
            // Retornamos la vista directamente como plantilla
            return View();
        }
    }
}