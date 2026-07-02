using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

public class AlimentosController : Controller
{
    private readonly AppDbContext _context;

    public AlimentosController(AppDbContext context)
    {
        _context = context;
    }

    // LISTADO: Usamos AsNoTracking para mayor velocidad en lectura
    public async Task<IActionResult> Index()
    {
        var lista = await _context.AlimentosBovinos.AsNoTracking().ToListAsync();
        return View(lista);
    }

    // GET: AddForm (Crear/Editar)
    public async Task<IActionResult> AddForm(int? id)
    {
        if (id == null) return View(new AlimentoBovino());

        var alimento = await _context.AlimentosBovinos.FindAsync(id);
        if (alimento == null) return NotFound();

        return View(alimento);
    }

    // POST: AddForm (Guardar con manejo de concurrencia y errores)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddForm(int id, AlimentoBovino alimento)
    {
        if (id != alimento.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                if (alimento.Id == 0)
                {
                    _context.Add(alimento);
                }
                else
                {
                    _context.Update(alimento);
                }
                await _context.SaveChangesAsync();

                // TempData para mostrar "Éxito" en la vista (Flash message)
                TempData["Mensaje"] = "Alimento guardado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "No se pudo guardar. Intente de nuevo.");
            }
        }
        return View(alimento);
    }

    // DELETE: Agregar eliminación segura
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var alimento = await _context.AlimentosBovinos.FindAsync(id);
        if (alimento != null)
        {
            _context.AlimentosBovinos.Remove(alimento);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}