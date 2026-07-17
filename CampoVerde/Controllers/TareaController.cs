using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class TareaController : Controller
    {
        private readonly AppDbContext _context;

        public TareaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tarea/Index
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (rol == "SUPER_ADMINISTRADOR")
            {
                return View(await _context.Tareas
                    .Include(t => t.Animal)
                    .Include(t => t.Cliente)
                    .ToListAsync());
            }

            return View(await _context.Tareas
                .Include(t => t.Animal)
                .Where(t => t.ClienteId == clienteId)
                .ToListAsync());
        }


        // GET: Tarea/Create
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


        // POST: Tarea/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            ModelState.Remove("Animal");

            if (ModelState.IsValid)
            {
                var clienteId = HttpContext.Session.GetInt32("ClienteId");

                if (clienteId == null)
                {
                    ModelState.AddModelError("", "El usuario no tiene un cliente asignado.");
                    return View(tarea);
                }

                tarea.ClienteId = clienteId.Value;

                tarea.FechaVencimiento =
                    DateTime.SpecifyKind(tarea.FechaVencimiento, DateTimeKind.Utc);

                _context.Tareas.Add(tarea);

                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se registró una nueva tarea: {tarea.Descripcion}",
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
                tarea.IdAnimal);

            return View(tarea);
        }

        // GET: Tarea/Edit
        public IActionResult Edit(int id)
        {
            var tarea = _context.Tareas.Find(id);

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (rol == "SUPER_ADMINISTRADOR")
            {
                ViewBag.IdAnimal = new SelectList(
                    _context.Animales,
                    "IdAnimal",
                    "nombre",
                    tarea.IdAnimal);
            }
            else
            {
                ViewBag.IdAnimal = new SelectList(
                    _context.Animales.Where(a => a.ClienteId == clienteId),
                    "IdAnimal",
                    "nombre",
                    tarea.IdAnimal);
            }

            if (tarea == null)
                return NotFound();

            ViewBag.IdAnimal = new SelectList(
                _context.Animales,
                "IdAnimal",
                "nombre",
                tarea.IdAnimal
            );

            return View(tarea);
        }

        // POST: Tarea/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarea tarea)
        {
            if (id != tarea.IdTarea)
                return NotFound();

            ModelState.Remove("Animal");

            if (!ModelState.IsValid)
            {
                ViewBag.IdAnimal = new SelectList(
                    _context.Animales,
                    "IdAnimal",
                    "nombre",
                    tarea.IdAnimal);

                return View(tarea);
            }

            var tareaDb = await _context.Tareas.FindAsync(id);

            if (tareaDb == null)
                return NotFound();

            tareaDb.Descripcion = tarea.Descripcion;
            tareaDb.IdAnimal = tarea.IdAnimal;
            tareaDb.FechaVencimiento = DateTime.SpecifyKind(
                tarea.FechaVencimiento,
                DateTimeKind.Utc);

            tareaDb.Completada = tarea.Completada;
            tareaDb.Prioridad = tarea.Prioridad;
            tareaDb.estado = tarea.estado;
            tareaDb.Encargado = tarea.Encargado ?? tareaDb.Encargado ?? "";
            tareaDb.Notas = tarea.Notas ?? tareaDb.Notas ?? "";

            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId != null)
            {
                tareaDb.ClienteId = clienteId.Value;
            }

            // Notificación
            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se actualizó la tarea: {tareaDb.Descripcion}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Tarea> consulta = _context.Tareas
                .Include(t => t.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(t => t.ClienteId == clienteId);
            }

            var tarea = await consulta.FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null)
                return NotFound();

            return View(tarea);
        }

        // GET: Tarea/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Tarea> consulta = _context.Tareas;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(t => t.ClienteId == clienteId);
            }

            var tarea = await consulta.FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null)
                return NotFound();

            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se eliminó la tarea: {tarea.Descripcion}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });

            _context.Tareas.Remove(tarea);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}