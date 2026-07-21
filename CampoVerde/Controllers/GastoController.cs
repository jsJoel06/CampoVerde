using CampoVerde.Data;
using CampoVerde.Models;
using CampoVerde.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    [AuthorizeRole(
    Permisos.SuperAdministrador,
    Permisos.Administrador)]
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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Gasto> consulta = _context.Gastos
                .Include(g => g.Animal);



            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(g => g.ClienteId == clienteId);
            }

            return View(await consulta.ToListAsync());
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Animal> animales = _context.Animales;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId);
            }

            ViewData["IdAnimal"] = new SelectList(
                animales.ToList(),
                "IdAnimal",
                "nombre");

            return View();
        }


        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gasto gasto)
        {
            ModelState.Remove("Animal");

            gasto.Fecha = DateTime.UtcNow;

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Animal> animales = _context.Animales;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId);
            }

            if (ModelState.IsValid)
            {
                _context.Gastos.Add(gasto);

                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se registró un nuevo gasto: {gasto.Concepto}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdAnimal"] = new SelectList(
                animales.ToList(),
                "IdAnimal",
                "nombre",
                gasto.IdAnimal);

            return View(gasto);
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Gasto> consulta = _context.Gastos;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(g => g.ClienteId == clienteId);
            }

            var gasto = await consulta.FirstOrDefaultAsync(g => g.IdGasto == id);

            if (gasto == null)
                return NotFound();

            IQueryable<Animal> animales = _context.Animales;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId);
            }

            ViewData["IdAnimal"] = new SelectList(
                animales.ToList(),
                "IdAnimal",
                "nombre",
                gasto.IdAnimal);

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

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Animal> animales = _context.Animales;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    IQueryable<Gasto> consulta = _context.Gastos
                        .Include(g => g.Animal);

                    if (rol != "SUPER_ADMINISTRADOR")
                    {
                        consulta = consulta.Where(g => g.ClienteId == clienteId);
                    }

                    var gastoExistente = await consulta
                        .FirstOrDefaultAsync(g => g.IdGasto == id);

                    if (gastoExistente == null)
                        return NotFound();

                    gastoExistente.Fecha = DateTime.SpecifyKind(
                        gasto.Fecha,
                        DateTimeKind.Utc);

                    gastoExistente.Concepto = gasto.Concepto;
                    gastoExistente.Monto = gasto.Monto;
                    gastoExistente.Categoria = gasto.Categoria;
                    gastoExistente.IdAnimal = gasto.IdAnimal;
                    gastoExistente.Notas = gasto.Notas;

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

            ViewData["IdAnimal"] = new SelectList(
                animales.ToList(),
                "IdAnimal",
                "nombre",
                gasto.IdAnimal);

            return View(gasto);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Gasto> consulta = _context.Gastos
                .Include(g => g.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(g => g.ClienteId == clienteId);
            }

            var gasto = await consulta.FirstOrDefaultAsync(g => g.IdGasto == id);

            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Gasto> consulta = _context.Gastos
                .Include(g => g.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(g => g.ClienteId == clienteId);
            }

            var gasto = await consulta.FirstOrDefaultAsync(g => g.IdGasto == id);

            if (gasto == null)
                return NotFound();

            return View(gasto);
        }

        // DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Gasto> consulta = _context.Gastos
                .Include(g => g.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(g => g.ClienteId == clienteId);
            }

            var gasto = await consulta.FirstOrDefaultAsync(g => g.IdGasto == id);

            if (gasto == null)
                return NotFound();

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