using CampoVerde.Data;
using CampoVerde.Models;
using CampoVerde.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


[AuthorizeRole(
    Permisos.SuperAdministrador,
    Permisos.Administrador,
    Permisos.Veterinario,
    Permisos.Operario
    )]
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
        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        if (rol == "SUPER_ADMINISTRADOR")
        {
            return View(new List<Vacuna>());
        }

        return View(await _context.Vacunas
            .Include(v => v.Animal)
            .Where(v => v.ClienteId == clienteId)
            .ToListAsync());
    }


    // GET: VACUNAS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        IQueryable<Vacuna> consulta = _context.Vacunas
            .Include(v => v.Animal);

        if (rol != "SUPER_ADMINISTRADOR")
        {
            consulta = consulta.Where(v => v.ClienteId == clienteId);
        }

        var vacuna = await consulta.FirstOrDefaultAsync(v => v.IdVacuna == id);

        if (vacuna == null)
            return NotFound();

        return View(vacuna);
    }

    // GET: VACUNAS/Create
    public IActionResult Create()
    {
        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        if (rol == "SUPER_ADMINISTRADOR")
        {
            ViewBag.IdAnimal = new SelectList(
                _context.Animales,
                "IdAnimal",
                "nombre");
        }
        else
        {
            ViewBag.IdAnimal = new SelectList(
                _context.Animales.Where(a => a.ClienteId == clienteId),
                "IdAnimal",
                "nombre");
        }

        return View();
    }


    // POST: VACUNAS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("IdAnimal,nombreVacuna,fechaAplicacion,frecuenciaMeses,observaciones")] Vacuna vacuna)
    {
        if (ModelState.IsValid)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
            {
                return Unauthorized();
            }

            vacuna.ClienteId = clienteId.Value;

            vacuna.fechaAplicacion = DateTime.SpecifyKind(
                vacuna.fechaAplicacion,
                DateTimeKind.Utc);

            vacuna.fechaProximaAplicacion = vacuna.fechaAplicacion.AddMonths(vacuna.frecuenciaMeses);

            _context.Vacunas.Add(vacuna);

            // Notificación
            _context.Notificaciones.Add(new Notificacion
            {
                ClienteId = clienteId.Value,
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

        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        if (rol == "SUPER_ADMINISTRADOR")
        {
            ViewBag.IdAnimal = new SelectList(
                _context.Animales,
                "IdAnimal",
                "nombre",
                vacuna.IdAnimal);
        }
        else
        {
            ViewBag.IdAnimal = new SelectList(
                _context.Animales.Where(a => a.ClienteId == clienteId),
                "IdAnimal",
                "nombre",
                vacuna.IdAnimal);
        }

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

        var clienteId = HttpContext.Session.GetInt32("ClienteId");
        vacuna.ClienteId = clienteId.Value;

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(vacuna);

                // Notificación
                _context.Notificaciones.Add(new Notificacion
                {
                    ClienteId = clienteId.Value,
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
            .Include(v => v.Animal)
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
        if (idvacuna == null)
            return NotFound();

        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        IQueryable<Vacuna> consulta = _context.Vacunas;

        if (rol != "SUPER_ADMINISTRADOR")
        {
            consulta = consulta.Where(v => v.ClienteId == clienteId);
        }

        var vacuna = await consulta.FirstOrDefaultAsync(v => v.IdVacuna == idvacuna);

        if (vacuna == null)
            return NotFound();

        _context.Notificaciones.Add(new Notificacion
        {
            ClienteId = clienteId.Value,
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

        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        var vacunas = _context.Vacunas
            .Where(v => v.IdAnimal == idAnimal);

        if (rol != "SUPER_ADMINISTRADOR")
        {
            vacunas = vacunas.Where(v => v.ClienteId == clienteId);
        }

        return View(vacunas.ToList());
    }

    // PRÓXIMAS VACUNAS
    public IActionResult ProximasVacunas()
    {
        DateTime hoy = DateTime.Today;

        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        var consulta = _context.Vacunas.Where(v =>
            v.fechaProximaAplicacion >= hoy &&
            v.fechaProximaAplicacion <= hoy.AddDays(30));

        if (rol != "SUPER_ADMINISTRADOR")
        {
            consulta = consulta.Where(v => v.ClienteId == clienteId);
        }

        return View(consulta
            .OrderBy(v => v.fechaProximaAplicacion)
            .ToList());
    }
}