using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class PartoController : Controller
    {
        private readonly AppDbContext _context;

        public PartoController(AppDbContext context)
        {
            _context = context;
        }

        //=========================
        // INDEX
        //=========================
        public async Task<IActionResult> Index()
        {
            var partos = await _context.Partos
                .Include(p => p.Animal)
                .OrderByDescending(p => p.FechaParto)
                .ToListAsync();

            ViewBag.TotalPartos = partos.Count;
            ViewBag.TotalMachos = partos.Count(x => x.SexoCria == "Macho");
            ViewBag.TotalHembras = partos.Count(x => x.SexoCria == "Hembra");
            ViewBag.PartosHoy = partos.Count(x => x.FechaParto.Date == DateTime.Today);

            return View(partos);
        }

        //=========================
        // HISTORIAL
        //=========================
        public async Task<IActionResult> Historial()
        {
            var historial = await _context.Partos
                .Include(p => p.Animal)
                .OrderByDescending(p => p.FechaParto)
                .ToListAsync();

            return View(historial);
        }

        //=========================
        // DETAILS
        //=========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var parto = await _context.Partos
                .Include(p => p.Animal)
                .FirstOrDefaultAsync(x => x.IdParto == id);

            if (parto == null)
                return NotFound();

            return View(parto);
        }

        //=========================
        // CREATE
        //=========================
        public IActionResult Create()
        {
            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Parto parto)
        {
            if (ModelState.IsValid)
            {
                // Fecha del parto
                parto.FechaParto = DateTime.UtcNow;

                // Obtener la madre
                var madre = await _context.Animales
                    .FirstOrDefaultAsync(a => a.IdAnimal == parto.IdAnimal);

                if (madre == null)
                {
                    ModelState.AddModelError("", "No se encontró el animal seleccionado.");

                    ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", parto.IdAnimal);
                    return View(parto);
                }

                // Registrar el parto
                _context.Partos.Add(parto);

                // Registrar automáticamente la cría
                var cria = new Animal
                {
                    codigo = parto.CodigoCria,
                    nombre = parto.NombreCria,
                    fechaNacimiento = parto.FechaParto,

                    // Hereda la raza de la madre
                    raza = madre.raza,

                    pesoActual = parto.PesoCria,

                    // Puedes cambiar este valor según tu enum
                    Estado = EstadoAnimal.ACTIVO,

                    // Si manejas lotes
                    lote = madre.lote
                };

                _context.Animales.Add(cria);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", parto.IdAnimal);

            return View(parto);
        }
        //=========================
        // EDIT
        //=========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var parto = await _context.Partos.FindAsync(id);

            if (parto == null)
                return NotFound();

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", parto.IdAnimal);

            return View(parto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Parto parto)
        {
            if (id != parto.IdParto)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Partos.Any(e => e.IdParto == parto.IdParto))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", parto.IdAnimal);

            return View(parto);
        }

        //=========================
        // DELETE
        //=========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var parto = await _context.Partos
                .Include(p => p.Animal)
                .FirstOrDefaultAsync(x => x.IdParto == id);

            if (parto == null)
                return NotFound();

            return View(parto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parto = await _context.Partos.FindAsync(id);

            if (parto != null)
            {
                _context.Partos.Remove(parto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}