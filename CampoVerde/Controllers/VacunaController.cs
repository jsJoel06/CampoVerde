using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        if (id == null)
            return NotFound();

        var vacuna = await _context.Vacunas
            .FirstOrDefaultAsync(m => m.IdVacuna == id);

        if (vacuna == null)
            return NotFound();

        return View(vacuna);
    }

    // GET: VACUNAS/Create
    public IActionResult Create()
    {
        ViewBag.IdAnimal = new SelectList(_context.Animales, "IdAnimal", "nombre");
        return View();
    }

    // POST: VACUNAS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        if (ModelState.IsValid)
        {
            vacuna.fechaAplicacion = DateTime.SpecifyKind(
                vacuna.fechaAplicacion,
                DateTimeKind.Utc);

            vacuna.fechaProximaAplicacion = vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);

            _context.Vacunas.Add(vacuna);

            // Notificación
            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se registró una nueva vacuna: {vacuna.nombreVacuna}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        ViewBag.IdAnimal = new SelectList(
            _context.Animales,
            "IdAnimal",
            "nombre",
            vacuna.IdAnimal);

        return View(vacuna);
    }

    // GET: VACUNAS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var vacuna = await _context.Vacunas.FindAsync(id);

        if (vacuna == null)
            return NotFound();

        ViewBag.IdAnimal = new SelectList(
            _context.Animales,
            "IdAnimal",
            "nombre",
            vacuna.IdAnimal);

        return View(vacuna);
    }

    // POST: VACUNAS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdVacuna,IdAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        if (id != vacuna.IdVacuna)
            return NotFound();

        vacuna.fechaAplicacion = DateTime.SpecifyKind(
            vacuna.fechaAplicacion,
            DateTimeKind.Utc);

        vacuna.fechaProximaAplicacion =
            vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vacuna);

                // Notificación
                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se actualizó la vacuna: {vacuna.nombreVacuna}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                });

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacunaExists(vacuna.IdVacuna))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.IdAnimal = new SelectList(
            _context.Animales,
            "IdAnimal",
            "nombre",
            vacuna.IdAnimal);

        return View(vacuna);
    }

    // GET: VACUNAS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var vacuna = await _context.Vacunas
            .FirstOrDefaultAsync(m => m.IdVacuna == id);

        if (vacuna == null)
            return NotFound();

        return View(vacuna);
    }

    // POST: VACUNAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? idvacuna)
    {
        var vacuna = await _context.Vacunas.FindAsync(idvacuna);

        if (vacuna == null)
            return NotFound();

        // Notificación
        _context.Notificaciones.Add(new Notificacion
        {
            Mensaje = $"Se eliminó la vacuna: {vacuna.nombreVacuna}",
            Fecha = DateTime.UtcNow,
            Leida = false
        });

        _context.Vacunas.Remove(vacuna);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool VacunaExists(int idvacuna)
    {
        return _context.Vacunas.Any(e => e.IdVacuna == idvacuna);
    }

    // HISTORIAL
    public IActionResult Historial(int? idAnimal)
    {
        if (idAnimal == null)
            return NotFound();

        var vacunas = _context.Vacunas
            .Where(v => v.IdAnimal == idAnimal)
            .ToList();

        return View(vacunas);
    }

    // PRÓXIMAS VACUNAS
    public IActionResult ProximasVacunas()
    {
        DateTime hoy = DateTime.Today;

        var proximas = _context.Vacunas
            .Where(v => v.fechaProximaAplicacion >= hoy &&
                        v.fechaProximaAplicacion <= hoy.AddDays(30))
            .OrderBy(v => v.fechaProximaAplicacion)
            .ToList();

        return View(proximas);
    }
}