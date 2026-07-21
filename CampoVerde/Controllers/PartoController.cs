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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Parto> consulta = _context.Partos.Include(p => p.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var partos = await consulta
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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Parto> consulta = _context.Partos.Include(p => p.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var historial = await consulta
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

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Parto> consulta = _context.Partos.Include(p => p.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var parto = await consulta.FirstOrDefaultAsync(x => x.IdParto == id);

            if (parto == null)
                return NotFound();

            return View(parto);
        }


        //=========================
        // CREATE
        //=========================
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            var animales = _context.Animales.AsQueryable();

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId);
            }

            ViewData["IdAnimal"] = new SelectList(animales, "IdAnimal", "nombre");

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
                // Obtener la madre
                var madre = await _context.Animales
                    .FirstOrDefaultAsync(a => a.IdAnimal == parto.IdAnimal);

                if (madre == null)
                {
                    ModelState.AddModelError("", "No se encontró el animal seleccionado.");

                    ViewData["IdAnimal"] = new SelectList(_context.Animales, "IdAnimal", "nombre", parto.IdAnimal);
                    return View(parto);
                }
                var clienteId = HttpContext.Session.GetInt32("ClienteId");
                // Cambiar estado de la madre después del parto
                madre.Estado = EstadoAnimal.EN_REPRODUCCION;
                madre.FechaEmbarazo = null;


                if (clienteId == null)
                {
                    return Unauthorized();
                }

                parto.ClienteId = clienteId.Value;

                // Registrar el parto
                _context.Partos.Add(parto);

                // Registrar automáticamente la cría
                var cria = new Animal
                {
                    codigo = parto.CodigoCria,
                    nombre = parto.NombreCria,
                    fechaNacimiento = parto.FechaParto,
                    raza = madre.raza,
                    pesoActual = parto.PesoCria,
                    Estado = EstadoAnimal.ACTIVO,
                    lote = madre.lote,
                    ClienteId = madre.ClienteId
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

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Parto> consulta = _context.Partos;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var parto = await consulta.FirstOrDefaultAsync(p => p.IdParto == id);

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
                    var clienteId = HttpContext.Session.GetInt32("ClienteId");
                    parto.ClienteId = clienteId.Value;

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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Parto> consulta = _context.Partos.Include(p => p.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var parto = await consulta.FirstOrDefaultAsync(x => x.IdParto == id);

            if (parto == null)
                return NotFound();

            return View(parto);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Parto> consulta = _context.Partos;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var parto = await consulta.FirstOrDefaultAsync(p => p.IdParto == id);

            if (parto == null)
                return NotFound();

            _context.Partos.Remove(parto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}