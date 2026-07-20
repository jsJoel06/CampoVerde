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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            if (rol == "SUPER_ADMINISTRADOR")
            {
                var todosLosAnimales = await _context.Animales
                    .Include(a => a.Cliente)
                    .ToListAsync();

                return View(todosLosAnimales);
            }


            var animalesCliente = await _context.Animales
                .Include(a => a.Cliente)
                .Where(a => a.ClienteId == clienteId)
                .ToListAsync();


            return View(animalesCliente);
        }
        // GET: Animal/Create
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return View(new Animal());
            }

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            Animal? animal;

            if (rol == "SUPER_ADMINISTRADOR")
            {
                animal = await _context.Animales
                    .FirstOrDefaultAsync(a => a.IdAnimal == id);
            }
            else
            {
                animal = await _context.Animales
                    .FirstOrDefaultAsync(a =>
                        a.IdAnimal == id &&
                        a.ClienteId == clienteId);
            }

            if (animal == null)
                return NotFound();

            return View(animal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animal animal, IFormFile? imagenArchivo)
        {
            ModelState.Remove("Cliente");
            ModelState.Remove("ClienteId");

            var clienteIds = HttpContext.Session.GetInt32("ClienteId");

            Console.WriteLine("ClienteId de la sesión: " + clienteIds);


            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"{item.Key}: {error.ErrorMessage}");
                    }
                }

                return View(animal);
            }

            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
            {
                ModelState.AddModelError("", "No existe ClienteId en sesión.");
                return View(animal);
            }

            animal.ClienteId = clienteId.Value;

            if (imagenArchivo != null && imagenArchivo.Length > 0)
            {
                var carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/ganado");

                Directory.CreateDirectory(carpeta);

                var nombre = Guid.NewGuid() +
                             Path.GetExtension(imagenArchivo.FileName);

                using var stream = new FileStream(
                    Path.Combine(carpeta, nombre),
                    FileMode.Create);

                await imagenArchivo.CopyToAsync(stream);

                animal.imagen = "/images/ganado/" + nombre;
            }

            if (animal.fechaNacimiento.HasValue)
            {
                animal.fechaNacimiento = DateTime.SpecifyKind(
                    animal.fechaNacimiento.Value,
                    DateTimeKind.Utc);
            }

            _context.Animales.Add(animal);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        


        // GET: Animal/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var rol = HttpContext.Session.GetString("Rol");


            var animal = await _context.Animales
                .FirstOrDefaultAsync(a =>
                    a.IdAnimal == id &&
                    (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
                );


            if (animal == null)
                return NotFound();


            return View(animal);
        }

        

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
                    var clienteId = HttpContext.Session.GetInt32("ClienteId");
                    var rol = HttpContext.Session.GetString("Rol");


                    var animalExistente = await _context.Animales
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a =>
                            a.IdAnimal == id &&
                            (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
                        );

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
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var rol = HttpContext.Session.GetString("Rol");


            var animal = await _context.Animales
                .FirstOrDefaultAsync(a =>
                    a.IdAnimal == id &&
                    (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
                );

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
                var clienteId = HttpContext.Session.GetInt32("ClienteId");
                var rol = HttpContext.Session.GetString("Rol");


                var animal = await _context.Animales
                    .FirstOrDefaultAsync(a =>
                        a.IdAnimal == id &&
                        (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
                    );

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

            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var rol = HttpContext.Session.GetString("Rol");


            var animal = await _context.Animales
                .FirstOrDefaultAsync(a =>
                    a.IdAnimal == id &&
                    (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
                );

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