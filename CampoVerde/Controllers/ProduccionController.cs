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
    Permisos.Administrador,
    Permisos.Operario)]
    public class ProduccionController : Controller
    {
        private readonly AppDbContext _context;

        public ProduccionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Produccion
        public async Task<IActionResult> Index(string filtro = "total")
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            IQueryable<Produccion> consulta = _context.Producciones
                .Include(p => p.Animal);



            // FILTRO POR CLIENTE

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta
                    .Where(p => p.ClienteId == clienteId);
            }



            var hoy = DateTime.UtcNow.Date;



            // =========================
            // FILTROS DE PRODUCCIÓN
            // =========================

            switch (filtro)
            {

                case "hoy":

                    consulta = consulta.Where(p =>
                        p.fechaProduccion.Date == hoy);

                    break;



                case "semana":

                    var inicioSemana = hoy.AddDays(
                        -(int)hoy.DayOfWeek + 1
                    );


                    consulta = consulta.Where(p =>
                        p.fechaProduccion.Date >= inicioSemana &&
                        p.fechaProduccion.Date <= hoy);

                    break;



                case "mes":

                    consulta = consulta.Where(p =>
                        p.fechaProduccion.Month == hoy.Month &&
                        p.fechaProduccion.Year == hoy.Year);

                    break;



                case "total":

                default:

                    break;

            }



            var producciones = await consulta
                .OrderByDescending(p => p.fechaProduccion)
                .ToListAsync();



            // =========================
            // ESTADÍSTICAS
            // =========================


            ViewBag.FiltroActual = filtro;


            ViewBag.TotalProduccion =
                producciones.Sum(p => p.cantidadLeche);



            ViewBag.TotalRegistros =
                producciones.Count;



            ViewBag.Promedio =
                producciones.Any()
                ? producciones.Average(p => p.cantidadLeche)
                : 0;



            ViewBag.ProduccionHoy =
                producciones
                .Where(p =>
                    p.fechaProduccion.Date == hoy)
                .Sum(p => p.cantidadLeche);



            ViewBag.TotalAnimales =
                producciones
                .Select(p => p.IdAnimal)
                .Distinct()
                .Count();



            return View(producciones);
        }


        // GET: Produccion/Create
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produccion produccion)
        {
            if (ModelState.IsValid)
            {

                var clienteId = HttpContext.Session.GetInt32("ClienteId");

                if (clienteId == null)
                {
                    ModelState.AddModelError("", "El usuario no tiene un cliente asignado.");
                    return View(produccion);
                }

                produccion.ClienteId = clienteId.Value;
                try
                {
                    // Asignar la fecha y hora actual
                    produccion.fechaProduccion = DateTime.UtcNow;

                    _context.Add(produccion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var error = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", error);
                }
            }

            ViewBag.IdAnimal = new SelectList(_context.Animales, "IdAnimal", "nombre", produccion.IdAnimal);
            return View(produccion);
        }


        // GET: Produccion/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var produccion = await _context.Producciones.FindAsync(id);

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (rol == "SUPER_ADMINISTRADOR")
            {
                ViewBag.IdAnimal = new SelectList(
                    _context.Animales,
                    "IdAnimal",
                    "nombre",
                    produccion.IdAnimal);
            }
            else
            {
                ViewBag.IdAnimal = new SelectList(
                    _context.Animales.Where(a => a.ClienteId == clienteId),
                    "IdAnimal",
                    "nombre",
                    produccion.IdAnimal);
            }

            return View(produccion);


        }


        // POST: Produccion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produccion produccion)
        {
            if (id != produccion.IdProduccion)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var clienteId = HttpContext.Session.GetInt32("ClienteId");

                    if (clienteId != null)
                    {
                        produccion.ClienteId = clienteId.Value;
                    }

                    _context.Update(produccion);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Producciones.Any(x => x.IdProduccion == id))
                        return NotFound();

                    throw;
                }
            }

            ViewBag.IdAnimal = new SelectList(
                _context.Animales,
                "IdAnimal",
                "nombre",
                produccion.IdAnimal);

            return View(produccion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Produccion> consulta = _context.Producciones
                .Include(p => p.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var produccion = await consulta.FirstOrDefaultAsync(p => p.IdProduccion == id);

            if (produccion == null)
                return NotFound();

            return View(produccion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Produccion> consulta = _context.Producciones;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            var produccion = await consulta.FirstOrDefaultAsync(p => p.IdProduccion == id);

            if (produccion == null)
                return NotFound();

            _context.Producciones.Remove(produccion);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Historial(int? idAnimal)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            var consulta = _context.Producciones
                .Include(p => p.Animal)
                .AsQueryable();

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(p => p.ClienteId == clienteId);
            }

            if (idAnimal.HasValue)
            {
                consulta = consulta.Where(p => p.IdAnimal == idAnimal);
            }

            return View(await consulta
                .OrderByDescending(p => p.fechaProduccion)
                .ToListAsync());
        }

    }
}