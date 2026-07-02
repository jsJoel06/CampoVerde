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
            var tareas = await _context.Tareas.Include(t => t.Animal).ToListAsync();
            return View(tareas);
        }

        // GET: Tarea/Create
        // GET: Tarea/Create
        public IActionResult Create()
        {
            ViewBag.IdAnimal = new SelectList(_context.Animales, "IdAnimal", "nombre");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            ModelState.Remove("Animal");

            if (ModelState.IsValid)
            {
                tarea.FechaVencimiento =
                    DateTime.SpecifyKind(tarea.FechaVencimiento, DateTimeKind.Utc);

                _context.Tareas.Add(tarea);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdAnimal = new SelectList(_context.Animales,
                "IdAnimal",
                "nombre",
                tarea.IdAnimal);

            return View(tarea);
        }

        public IActionResult Edit(int id)
        {
            var tarea = _context.Tareas.Find(id);

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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarea tarea)
        {
            if (id != tarea.IdTarea)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.IdAnimal = new SelectList(_context.Animales, "IdAnimal", "nombre", tarea.IdAnimal);
                return View(tarea);
            }

            var tareaDb = await _context.Tareas.FindAsync(id);

            if (tareaDb == null)
                return NotFound();

            tareaDb.Descripcion = tarea.Descripcion;
            tareaDb.IdAnimal = tarea.IdAnimal;

            tareaDb.FechaVencimiento = DateTime.SpecifyKind(
                tarea.FechaVencimiento,
                DateTimeKind.Utc
            );

            tareaDb.Completada = tarea.Completada;
            tareaDb.Prioridad = tarea.Prioridad;
            tareaDb.estado = tarea.estado;

            // 🔥 FIX REAL: nunca dejar NULL
            tareaDb.Encargado = tarea.Encargado ?? tareaDb.Encargado ?? "";
            tareaDb.Notas = tarea.Notas ?? tareaDb.Notas ?? "";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var tarea = await _context.Tareas
                .Include(t => t.Animal) 
                .FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null)
                return NotFound();

            return View(tarea);
        }


        // GET: Tarea/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}