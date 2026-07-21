using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");


        if (rol == "SUPER_ADMINISTRADOR")
        {
            var todos = await _context.AlimentosBovinos
                .Include(a => a.Cliente)
                .AsNoTracking()
                .ToListAsync();

            return View(todos);
        }


        var lista = await _context.AlimentosBovinos
            .Where(a => a.ClienteId == clienteId)
            .AsNoTracking()
            .ToListAsync();


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

                    var clienteId = HttpContext.Session.GetInt32("ClienteId");

                    if (clienteId == null || clienteId == 0)
                    {
                        ModelState.AddModelError("", "El usuario no tiene un cliente asignado.");
                        return View(alimento);
                    }


                    alimento.ClienteId = clienteId.Value;

                    // Crear alimento
                    _context.AlimentosBovinos.Add(alimento);

                    decimal costoTotal = alimento.CostoUnitario * alimento.CantidadDisponible;

                    _context.Gastos.Add(new Gasto
                    {
                        Categoria = CategoriaGasto.Alimentacion,
                        ClienteId = clienteId.Value,
                        Concepto = $"Compra de alimento: {alimento.Nombre}",
                        Fecha = DateTime.UtcNow,
                        Monto = alimento.CostoUnitario,
                        Notas = "Registro automático"
                    });

                    Console.WriteLine($"Gastos en memoria: {_context.Gastos.Local.Count}");

                    Console.WriteLine("Creando gasto...");

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

                    var clienteId = HttpContext.Session.GetInt32("ClienteId");

                    if (clienteId == null || clienteId == 0)
                    {
                        ModelState.AddModelError("", "El usuario no tiene un cliente asignado.");
                        return View(alimento);
                    }

                    Console.WriteLine("Gasto agregado al contexto.");

                    alimento.ClienteId = clienteId.Value;

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

        var clienteId = HttpContext.Session.GetInt32("ClienteId");
        var rol = HttpContext.Session.GetString("Rol");


        var alimento = await _context.AlimentosBovinos
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
            );

        if (alimento == null)
            return NotFound();

        return View(alimento);
    }

    // ==========================
    // HISTORIAL
    // ==========================
    public async Task<IActionResult> Historial()
    {
        var clienteId = HttpContext.Session.GetInt32("ClienteId");
        var rol = HttpContext.Session.GetString("Rol");


        var historial = await _context.AlimentosBovinos
            .Where(a =>
                rol == "SUPER_ADMINISTRADOR" ||
                a.ClienteId == clienteId
            )
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
        var clienteId = HttpContext.Session.GetInt32("ClienteId");
        var rol = HttpContext.Session.GetString("Rol");


        var alimento = await _context.AlimentosBovinos
            .FirstOrDefaultAsync(a =>
                a.Id == id &&
                (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
            );

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