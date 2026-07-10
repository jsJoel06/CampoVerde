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

                // --- INICIO DE NOTIFICACIÓN AUTOMÁTICA ---
                var nuevaNotif = new CampoVerde.Models.Notificacion
                {
                    Mensaje = $"Se ha registrado un nuevo animal: {animal.nombre}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                };
                _context.Add(nuevaNotif);
                // --- FIN DE NOTIFICACIÓN ---

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

        // POST: Animal/Edit/5
        // POST: Animal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Animal animal, IFormFile? imagenArchivo)
        {
            if (id != animal.IdAnimal)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el registro actual
                    var animalExistente = await _context.Animales
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.IdAnimal == id);

                    if (animalExistente == null)
                        return NotFound();

                    // Manejo de imagen
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
                        animal.imagen = animalExistente.imagen;
                    }

                    // Fecha UTC
                    if (animal.fechaNacimiento.HasValue)
                    {
                        animal.fechaNacimiento = DateTime.SpecifyKind(
                            animal.fechaNacimiento.Value,
                            DateTimeKind.Utc);
                    }

                    // Actualizar animal
                    _context.Animales.Update(animal);

                    // Agregar notificación
                    _context.Notificaciones.Add(new Notificacion
                    {
                        Mensaje = $"Se ha actualizado el animal: {animal.nombre}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });

                    // Guardar todo
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;
                }
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
            try
            {
                var animal = await _context.Animales.FindAsync(id);

                if (animal == null)
                    return NotFound();

                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se ha eliminado el animal: {animal.nombre}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                });

                _context.Animales.Remove(animal);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animales.FirstOrDefaultAsync(m => m.IdAnimal == id);
            if (animal == null) return NotFound();

            // Generar el código QR
            string urlDetalle = $"https://campoverde.onrender.com/Animal/Details/{animal.IdAnimal}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(urlDetalle, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            ViewBag.QrCodeUri = "data:image/png;base64," + Convert.ToBase64String(qrCodeImage);

            return View(animal);
        }
    }
}