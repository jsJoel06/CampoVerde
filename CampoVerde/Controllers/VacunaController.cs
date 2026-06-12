using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para Async, ToListAsync, etc.
using CampoVerde.Models;
using CampoVerde.Data;

public class VacunaController : Controller
{
    private readonly AppDbContext _context;

    public VacunaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: VACUNAS
    public async Task<IActionResult> Index()
    {
        return View(await _context.Vacunas.ToListAsync());
    }

    // GET: VACUNAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vacuna = await _context.Vacunas
            .FirstOrDefaultAsync(m => m.idVacuna == id);

        if (vacuna == null) return NotFound();

        return View(vacuna);
    }

    // GET: VACUNAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: VACUNAS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("idVacuna,idAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        // Validación: Si la fecha es mínima, forzamos la fecha de hoy para evitar el 01/01/0001
        if (vacuna.fechaAplicacion == DateTime.MinValue)
        {
            vacuna.fechaAplicacion = DateTime.Today;
        }

        if (ModelState.IsValid)
        {
            // Cálculo correcto
            vacuna.fechaProximaAplicacion = vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);

            _context.Add(vacuna);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(vacuna);
    }

    // GET: Vacunas/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vacuna = await _context.Vacunas.FindAsync(id);
        if (vacuna == null)
        {
            return NotFound();
        }

        // Aquí es donde se carga la vista Edit.cshtml con los datos actuales
        return View(vacuna);
    }

    // POST: VACUNAS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("idVacuna,idAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        if (id != vacuna.idVacuna) return NotFound();

        if (ModelState.IsValid)
        {
            vacuna.fechaProximaAplicacion = vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);
            try
            {
                _context.Update(vacuna);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacunaExists(vacuna.idVacuna)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(vacuna);
    }

    // GET: VACUNAS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var vacuna = await _context.Vacunas.FirstOrDefaultAsync(m => m.idVacuna == id);
        if (vacuna == null) return NotFound();

        return View(vacuna);


    }

     

    // POST: VACUNAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idvacuna)
    {
        var vacuna = await _context.Vacunas.FindAsync(idvacuna);
        if (vacuna != null) _context.Vacunas.Remove(vacuna);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool VacunaExists(int idvacuna)
    {
        return _context.Vacunas.Any(e => e.idVacuna == idvacuna);
    }

    public IActionResult Historial(int? idAnimal)
    {
        if (idAnimal == null) return NotFound();
        var vacunas = _context.Vacunas.Where(v => v.idAnimal == idAnimal).ToList();
        return View(vacunas);
    }

    public IActionResult ProximasVacunas()
    {
        DateTime hoy = DateTime.Today;
        var proximas = _context.Vacunas
                               .Where(v => v.fechaProximaAplicacion >= hoy && v.fechaProximaAplicacion <= hoy.AddDays(30))
                               .OrderBy(v => v.fechaProximaAplicacion)
                               .ToList();
        return View(proximas);
    }
}