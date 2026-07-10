using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class GastoController : Controller
    {
        private readonly AppDbContext _context;

        public GastoController(AppDbContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<IActionResult> Index()
        {
            var gastos = await _context.Gastos
                .Include(g => g.Animal)
                .ToListAsync();

            return View(gastos);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre");
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gasto gasto)
        {
            ModelState.Remove("Animal");

            gasto.Fecha = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                _context.Gastos.Add(gasto);

                // Notificación
                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se registró un nuevo gasto: {gasto.Concepto}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", gasto.IdAnimal);
            return View(gasto);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var gasto = await _context.Gastos.FindAsync(id);

            if (gasto == null)
                return NotFound();

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", gasto.IdAnimal);

            return View(gasto);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Gasto gasto)
        {
            if (id != gasto.IdGasto)
                return NotFound();

            ModelState.Remove("Animal");

            if (ModelState.IsValid)
            {
                try
                {
                    var gastoExistente = await _context.Gastos.FindAsync(id);

                    if (gastoExistente == null)
                        return NotFound();

                    gastoExistente.Fecha = DateTime.SpecifyKind(gasto.Fecha, DateTimeKind.Utc);
                    gastoExistente.Concepto = gasto.Concepto;
                    gastoExistente.Monto = gasto.Monto;
                    gastoExistente.Categoria = gasto.Categoria;
                    gastoExistente.IdAnimal = gasto.IdAnimal;
                    gastoExistente.Notas = gasto.Notas;

                    // Notificación
                    _context.Notificaciones.Add(new Notificacion
                    {
                        Mensaje = $"Se actualizó el gasto: {gastoExistente.Concepto}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "Ocurrió un error al guardar los cambios.");
                }
            }

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", gasto.IdAnimal);
            return View(gasto);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var gasto = await _context.Gastos
                .Include(g => g.Animal)
                .FirstOrDefaultAsync(g => g.IdGasto == id);

            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var gasto = await _context.Gastos
                .Include(g => g.Animal)
                .FirstOrDefaultAsync(g => g.IdGasto == id);

            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gasto = await _context.Gastos.FindAsync(id);

            if (gasto == null)
                return NotFound();

            // Notificación
            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se eliminó el gasto: {gasto.Concepto}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });

            _context.Gastos.Remove(gasto);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}