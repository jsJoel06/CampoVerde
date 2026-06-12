using Microsoft.AspNetCore.Mvc;
using CampoVerde.Data;


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
        public async Task<IActionResult> subirFoto(int animalId, IFormFile fotos)
        {
            if( fotos != null && fotos.Length > 0)
            {
                var nombreArchivo = $"{animalId}_{Guid.NewGuid()}.jpg";
                var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", nombreArchivo);

                using (var stream = new FileStream(rutaGuardado, FileMode.Create))
                {
                    await fotos.CopyToAsync(stream);
                }
                var animal = await _context.Animales.FindAsync(animalId);
                animal.imagen = nombreArchivo;
                await _context.SaveChangesAsync();
               
            }

            return RedirectToAction("Detalles", "Animal", new { id = animalId });
        }

    }
}
