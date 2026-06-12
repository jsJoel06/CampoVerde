using Microsoft.AspNetCore.Mvc;
using CampoVerde.Data;
using Microsoft.EntityFrameworkCore;
using CampoVerde.Models;
using QRCoder;

namespace CampoVerde.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AppDbContext _context;

        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Animal
        public async Task<IActionResult> Index()
        {
            var animals = await _context.Animales.ToListAsync();
            return View(animals);
        }

       

        // GET: Animal/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Animal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animal animal, IFormFile? imagenArchivo)
        {
            if (ModelState.IsValid)
            {
                // 1. Manejo de la imagen
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagenArchivo.FileName);
                    // Asegúrate de que esta carpeta exista en wwwroot
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ganado", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imagenArchivo.CopyToAsync(stream);
                    }
                    animal.imagen = "/images/ganado/" + fileName;
                }

                // 2. Configuración de fecha
                if (animal.fechaNacimiento.HasValue)
                    animal.fechaNacimiento = DateTime.SpecifyKind(animal.fechaNacimiento.Value, DateTimeKind.Utc);

                // 3. Guardar en base de datos
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        // GET: Animal/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var animal = await _context.Animales.FindAsync(id);
            if (animal == null) return NotFound();
            return View(animal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Animal animal, IFormFile? imagenArchivo)
        {
            if (id != animal.idAnimal) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Obtener el registro actual para no perder la imagen si no se sube una nueva
                    var animalExistente = await _context.Animales.AsNoTracking().FirstOrDefaultAsync(a => a.idAnimal == id);

                    if (animalExistente != null)
                    {
                        // 2. Si se sube una nueva imagen, guardarla
                        if (imagenArchivo != null && imagenArchivo.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagenArchivo.FileName);
                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/ganado", fileName);

                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await imagenArchivo.CopyToAsync(stream);
                            }
                            animal.imagen = "/images/ganado/" + fileName;
                        }
                        else
                        {
                            // Mantener la imagen anterior si no se sube una nueva
                            animal.imagen = animalExistente.imagen;
                        }
                    }

                    if (animal.fechaNacimiento.HasValue)
                        animal.fechaNacimiento = DateTime.SpecifyKind(animal.fechaNacimiento.Value, DateTimeKind.Utc);

                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Animales.Any(e => e.idAnimal == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }


        // GET: Animal/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await _context.Animales.FindAsync(id);
            if (animal == null) return NotFound();
            return View(animal);
        }

        // POST: Animal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animales.FindAsync(id);
            if (animal != null)
            {
                _context.Animales.Remove(animal);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animales.FirstOrDefaultAsync(m => m.idAnimal == id);
            if (animal == null) return NotFound();

            // Generar el código QR
            // La URL debe apuntar a donde está el detalle de tu animal (ejemplo real)
            string urlDetalle = $"https://tu-sitio-ganadero.com/Animal/Details/{animal.idAnimal}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(urlDetalle, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            // Pasamos el QR a la vista como una imagen base64
            ViewBag.QrCodeUri = "data:image/png;base64," + Convert.ToBase64String(qrCodeImage);

            return View(animal);
        }


      
    }
}