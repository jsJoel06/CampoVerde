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

        // POST: Animal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animal animal, IFormFile? imagenArchivo)
        {
            if (ModelState.IsValid)
            {

                // 1. Obtener cliente del usuario logueado
                var clienteId = HttpContext.Session.GetInt32("ClienteId");


                if (clienteId == null || clienteId == 0)
                {
                    ModelState.AddModelError("", "El usuario no tiene un cliente asignado.");
                    return View(animal);
                }


                // Asignar dueño del animal
                animal.ClienteId = clienteId.Value;



                // 2. Manejo de imagen
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(imagenArchivo.FileName);


                    string path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/ganado",
                        fileName
                    );


                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imagenArchivo.CopyToAsync(stream);
                    }


                    animal.imagen = "/images/ganado/" + fileName;
                }



                // 3. Fecha UTC
                if (animal.fechaNacimiento.HasValue)
                {
                    animal.fechaNacimiento =
                        DateTime.SpecifyKind(
                            animal.fechaNacimiento.Value,
                            DateTimeKind.Utc
                        );
                }



                // 4. Guardar animal
                _context.Add(animal);



                // 5. Notificación
                var nuevaNotif = new CampoVerde.Models.Notificacion
                {
                    Mensaje = $"Se ha registrado un nuevo animal: {animal.nombre}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                };


                _context.Add(nuevaNotif);



                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            return View(animal);
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