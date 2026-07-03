using CampoVerde.Data; 
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class ProduccionController : Controller
    {
        private readonly AppDbContext _context;

        public ProduccionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Produccion
        public async Task<IActionResult> Index()
        {
            var producciones = await _context.Producciones
                .Include(p => p.Animal)
                .ToListAsync();

            var hoy = DateTime.Today;
            var manana = hoy.AddDays(1);

            ViewBag.TotalProduccion = producciones.Sum(p => p.cantidadLeche);
            ViewBag.TotalRegistros = producciones.Count;

            ViewBag.Promedio = producciones.Any()
                ? producciones.Average(p => p.cantidadLeche)
                : 0;

            // 🔥 PRODUCCIÓN DEL DÍA (ARREGLADA)
            ViewBag.ProduccionHoy = producciones
                .Where(p => p.fechaProduccion >= hoy && p.fechaProduccion < manana)
                .Sum(p => p.cantidadLeche);

            return View(producciones);
        }

        // GET: Produccion/Create
        public IActionResult Create()
        {
            // Cargamos los animales en el ViewBag para el SelectList
            ViewBag.IdAnimal = new SelectList(_context.Animales, "IdAnimal", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produccion produccion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Asignar la fecha y hora actual
                    produccion.fechaProduccion = DateTime.UtcNow;

                    _context.Add(produccion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var error = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", error);
                }
            }

            ViewBag.IdAnimal = new SelectList(_context.Animales, "IdAnimal", "nombre", produccion.IdAnimal);
            return View(produccion);
        }

        // GET: Produccion/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var produccion = await _context.Producciones.FindAsync(id);

            if (produccion == null)
                return NotFound();

            ViewBag.IdAnimal = new SelectList(
                _context.Animales,
                "IdAnimal",
                "nombre",
                produccion.IdAnimal);

            return View(produccion);
        }


        // POST: Produccion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produccion produccion)
        {
            if (id != produccion.IdProduccion)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produccion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Producciones.Any(x => x.IdProduccion == id))
                        return NotFound();

                    throw;
                }
            }

            ViewBag.IdAnimal = new SelectList(
                _context.Animales,
                "IdAnimal",
                "nombre",
                produccion.IdAnimal);

            return View(produccion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var produccion = await _context.Producciones
                .Include(p => p.Animal)
                .FirstOrDefaultAsync(p => p.IdProduccion == id);

            if (produccion == null)
                return NotFound();

            return View(produccion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produccion = await _context.Producciones.FindAsync(id);

            if (produccion != null)
            {
                _context.Producciones.Remove(produccion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Historial(int? idAnimal)
        {
            var consulta = _context.Producciones
                .Include(p => p.Animal)
                .AsQueryable();

            if (idAnimal.HasValue)
            {
                consulta = consulta.Where(p => p.IdAnimal == idAnimal);
            }

            return View(await consulta
                .OrderByDescending(p => p.fechaProduccion)
                .ToListAsync());
        }
    }
}