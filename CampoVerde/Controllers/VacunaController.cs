using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
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
            .FirstOrDefaultAsync(m => m.IdVacuna == id);

        if (vacuna == null) return NotFound();

        return View(vacuna);
    }

    // GET: VACUNAS/Create
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        // Forzamos la zona horaria antes de cualquier validación
        vacuna.fechaAplicacion = DateTime.SpecifyKind(vacuna.fechaAplicacion, DateTimeKind.Utc);

        // Cálculo de la fecha próxima
        vacuna.fechaProximaAplicacion = vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);

        if (ModelState.IsValid)
        {
            _context.Add(vacuna);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Si llega aquí, es porque algo falló en la validación
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
    // POST: VACUNAS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdVacuna,IdAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        if (id != vacuna.IdVacuna) return NotFound();

        // 1. Corregimos el KIND de la fecha antes de cualquier operación
        vacuna.fechaAplicacion = DateTime.SpecifyKind(vacuna.fechaAplicacion, DateTimeKind.Utc);

        // 2. Calculamos la próxima fecha
        vacuna.fechaProximaAplicacion = vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);

        if (ModelState.IsValid)
        {
            try
            {
                // 3. Importante: Para un Edit, lo ideal es obtener el registro actual 
                // para no sobrescribir campos que no están en el Bind (como fechaProximaAplicacion 
                // si fuera calculada en base de datos)
                _context.Update(vacuna);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacunaExists(vacuna.IdVacuna)) return NotFound();
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

        var vacuna = await _context.Vacunas.FirstOrDefaultAsync(m => m.IdVacuna == id);
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
        return _context.Vacunas.Any(e => e.IdVacuna == idvacuna);
    }

    public IActionResult Historial(int? idAnimal)
    {
        if (idAnimal == null) return NotFound();
        var vacunas = _context.Vacunas.Where(v => v.IdAnimal == idAnimal).ToList();
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