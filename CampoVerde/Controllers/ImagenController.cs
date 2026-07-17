using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class ImagenController : Controller
    {
        private readonly AppDbContext _context;

        public ImagenController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubirFoto(int animalId, IFormFile fotos)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Animal> consulta = _context.Animales;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(a => a.ClienteId == clienteId);
            }

            var animal = await consulta.FirstOrDefaultAsync(a => a.IdAnimal == animalId);

            if (animal == null)
                return NotFound();

            if (fotos != null && fotos.Length > 0)
            {
                string carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "imagenes");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                string extension = Path.GetExtension(fotos.FileName);

                string nombreArchivo = $"{animalId}_{Guid.NewGuid()}{extension}";

                string rutaGuardado = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                {
                    await fotos.CopyToAsync(stream);
                }

                animal.imagen = nombreArchivo;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Animal", new { id = animalId });
        }
    }
}