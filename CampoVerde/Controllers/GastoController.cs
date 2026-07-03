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

        public async Task<IActionResult> Index()
        {
            var gastos = await _context.Gastos.Include(g => g.Animal).ToListAsync();
            return View(gastos);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gasto gasto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gasto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", gasto.IdAnimal);
            return View(gasto);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto == null) return NotFound();

            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", gasto.IdAnimal);
            return View(gasto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Gasto gasto)
        {
            if (id != gasto.IdGasto) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(gasto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", gasto.IdAnimal);
            return View(gasto);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var gasto = await _context.Gastos.Include(g => g.Animal).FirstOrDefaultAsync(m => m.IdGasto == id);
            return gasto == null ? NotFound() : View(gasto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gasto = await _context.Gastos.FindAsync(id);
            _context.Gastos.Remove(gasto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}