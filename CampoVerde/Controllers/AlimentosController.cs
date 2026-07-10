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

    // LISTADO
    public async Task<IActionResult> Index()
    {
        var lista = await _context.AlimentosBovinos.AsNoTracking().ToListAsync();
        return View(lista);
    }

    // GET: AddForm
    public async Task<IActionResult> AddForm(int? id)
    {
        if (id == null)
            return View(new AlimentoBovino());

        var alimento = await _context.AlimentosBovinos.FindAsync(id);

        if (alimento == null)
            return NotFound();

        return View(alimento);
    }

    // POST: AddForm
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddForm(int id, AlimentoBovino alimento)
    {
        if (id != alimento.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                if (alimento.Id == 0)
                {
                    // Crear alimento
                    _context.AlimentosBovinos.Add(alimento);

                    // Notificación
                    _context.Notificaciones.Add(new Notificacion
                    {
                        Mensaje = $"Se registró un nuevo alimento: {alimento.Nombre}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });
                }
                else
                {
                    // Editar alimento
                    _context.AlimentosBovinos.Update(alimento);

                    // Notificación
                    _context.Notificaciones.Add(new Notificacion
                    {
                        Mensaje = $"Se actualizó el alimento: {alimento.Nombre}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });
                }

                await _context.SaveChangesAsync();

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

    // ==========================
    // DETAILS
    // ==========================
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var alimento = await _context.AlimentosBovinos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alimento == null)
            return NotFound();

        return View(alimento);
    }

    // ==========================
    // HISTORIAL
    // ==========================
    public async Task<IActionResult> Historial()
    {
        var historial = await _context.AlimentosBovinos
            .AsNoTracking()
            .OrderByDescending(a => a.Id)
            .ToListAsync();

        return View(historial);
    }

    // ==========================
    // DELETE (GET)
    // ==========================
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var alimento = await _context.AlimentosBovinos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (alimento == null)
            return NotFound();

        return View(alimento);
    }

    // POST: Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var alimento = await _context.AlimentosBovinos.FindAsync(id);

        if (alimento != null)
        {
            // Notificación
            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se eliminó el alimento: {alimento.Nombre}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });

            _context.AlimentosBovinos.Remove(alimento);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}