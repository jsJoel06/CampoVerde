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
        public async Task<IActionResult> Edit(
            int id,
            Animal animal,
            IFormFile? imagenArchivo)
        {

            if (id != animal.IdAnimal)
                return NotFound();



            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            var rol = HttpContext.Session.GetString("Rol");



            var animalExistente = await _context.Animales
                .FirstOrDefaultAsync(a =>
                    a.IdAnimal == id &&
                    (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
                );



            if (animalExistente == null)
                return NotFound();



            if (ModelState.IsValid)
            {

                try
                {


                    // =========================
                    // GUARDAR ESTADO ANTERIOR
                    // =========================

                    var estadoAnterior = animalExistente.Estado;



                    // =========================
                    // IMAGEN
                    // =========================

                    if (imagenArchivo != null && imagenArchivo.Length > 0)
                    {

                        string fileName =
                            Guid.NewGuid().ToString()
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



                        animalExistente.imagen =
                            "/images/ganado/" + fileName;

                    }





                    // =========================
                    // ACTUALIZAR DATOS
                    // =========================


                    animalExistente.nombre = animal.nombre;


                    animalExistente.codigo = animal.codigo;


                    animalExistente.raza = animal.raza;


                    animalExistente.pesoActual = animal.pesoActual;


                    animalExistente.lote = animal.lote;


                    animalExistente.Estado = animal.Estado;




                    if (animal.fechaNacimiento.HasValue)
                    {

                        animalExistente.fechaNacimiento =
                            DateTime.SpecifyKind(
                                animal.fechaNacimiento.Value,
                                DateTimeKind.Utc
                            );

                    }


                    // =========================
                    // CONTROL FECHA EMBARAZO
                    // =========================

                    if (animal.Estado == EstadoAnimal.EMBARAZADA)
                    {
                        if (animalExistente.FechaEmbarazo == null)
                        {
                            animalExistente.FechaEmbarazo = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        animalExistente.FechaEmbarazo = null;
                    }

                    // =========================
                    // NOTIFICACIÓN
                    // =========================


                    _context.Notificaciones.Add(new Notificacion
                    {

                        Mensaje =
                        $"Se actualizó el animal: {animalExistente.nombre}",


                        Fecha = DateTime.UtcNow,


                        Leida = false

                    });







                    await _context.SaveChangesAsync();



                    return RedirectToAction(nameof(Index));


                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);

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


        // GET: Animal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var rol = HttpContext.Session.GetString("Rol");

            var animal = await _context.Animales
                .Include(a => a.Cliente)
                .FirstOrDefaultAsync(a =>
                    a.IdAnimal == id &&
                    (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId));

            if (animal == null)
                return NotFound();

            // ==========================
            // ÚLTIMA VACUNA
            // ==========================

            var ultimaVacuna = await _context.Vacunas
                .Where(v => v.IdAnimal == animal.IdAnimal)
                .OrderByDescending(v => v.fechaAplicacion)
                .FirstOrDefaultAsync();

            ViewBag.UltimaVacuna = ultimaVacuna;
            ViewBag.Vacunado = ultimaVacuna != null;

            // ==========================
            // ÚLTIMA PRODUCCIÓN
            // ==========================

            var ultimaProduccion = await _context.Producciones
                .Where(p => p.IdAnimal == animal.IdAnimal)
                .OrderByDescending(p => p.fechaProduccion)
                .FirstOrDefaultAsync();

            ViewBag.UltimaProduccion = ultimaProduccion;

            // ==========================
            // TOTAL DE PRODUCCIONES
            // ==========================

            ViewBag.TotalProducciones = await _context.Producciones
                .CountAsync(p => p.IdAnimal == animal.IdAnimal);

            // ==========================
            // TOTAL DE LECHE
            // ==========================

            ViewBag.TotalLeche = await _context.Producciones
                .Where(p => p.IdAnimal == animal.IdAnimal)
                .SumAsync(p => (double?)p.cantidadLeche) ?? 0;

            // ==========================
            // TOTAL DE VACUNAS
            // ==========================

            ViewBag.TotalVacunas = await _context.Vacunas
                .CountAsync(v => v.IdAnimal == animal.IdAnimal);

            // ==========================
            // EDAD DEL ANIMAL
            // ==========================

            // ==========================
            // CALCULAR EDAD DEL ANIMAL
            // ==========================

            if (animal.fechaNacimiento.HasValue)
            {
                DateTime nacimiento = animal.fechaNacimiento.Value;
                DateTime hoy = DateTime.Today;

                int años = hoy.Year - nacimiento.Year;
                int meses = hoy.Month - nacimiento.Month;
                int dias = hoy.Day - nacimiento.Day;


                if (dias < 0)
                {
                    meses--;
                }


                if (meses < 0)
                {
                    años--;
                    meses += 12;
                }


                ViewBag.EdadAnimal = $"{años} años y {meses} meses";
            }
            else
            {
                ViewBag.EdadAnimal = "Fecha no registrada";
            }

            // ==========================
            // QR
            // ==========================

            string urlDetalle = $"https://campoverde.onrender.com/Animal/Details/{animal.IdAnimal}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrData = qrGenerator.CreateQrCode(urlDetalle, QRCodeGenerator.ECCLevel.Q);

            PngByteQRCode qrCode = new PngByteQRCode(qrData);

            byte[] bytes = qrCode.GetGraphic(20);

            ViewBag.QrCodeUri =
                $"data:image/png;base64,{Convert.ToBase64String(bytes)}";

            return View(animal);
        }
    }
}