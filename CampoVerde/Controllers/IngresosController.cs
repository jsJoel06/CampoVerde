using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class IngresosController : Controller
    {
        private readonly AppDbContext _context;

        public IngresosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ingresos
        public async Task<IActionResult> Index()
        {
            var ingresos = await _context.Ingresos
                .Include(i => i.Animal)
                .ToListAsync();

            return View(ingresos);
        }

        // GET: Ingresos/AddForm
        public IActionResult AddForm(int? id)
        {
            var listaAnimales = _context.Animales
                .OrderBy(a => a.nombre)
                .Select(a => new { a.IdAnimal, a.nombre })
                .ToList();

            ViewBag.Animales = new SelectList(listaAnimales, "IdAnimal", "nombre");

            if (id == null)
            {
                return View(new Ingreso
                {
                    Fecha = DateTime.UtcNow
                });
            }

            var ingreso = _context.Ingresos.Find(id);

            if (ingreso == null)
                return NotFound();

            return View(ingreso);
        }

        // POST: Guardar (Crear o Editar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Ingreso ingreso)
        {
            ModelState.Remove("Animal");

            ingreso.Fecha = DateTime.UtcNow;

            ViewBag.Animales = new SelectList(
                _context.Animales.ToList(),
                "IdAnimal",
                "nombre",
                ingreso.IdAnimal);

            if (!ModelState.IsValid)
            {
                return View("AddForm", ingreso);
            }

            try
            {
                if (ingreso.IdIngreso == 0)
                {
                    // Crear
                    _context.Ingresos.Add(ingreso);

                    _context.Notificaciones.Add(new Notificacion
                    {
                        Mensaje = $"Se registró un nuevo ingreso: {ingreso.Concepto}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });
                }
                else
                {
                    // Editar
                    var ingresoExistente = await _context.Ingresos.FindAsync(ingreso.IdIngreso);

                    if (ingresoExistente == null)
                        return NotFound();

                    _context.Entry(ingresoExistente).CurrentValues.SetValues(ingreso);

                    _context.Notificaciones.Add(new Notificacion
                    {
                        Mensaje = $"Se actualizó el ingreso: {ingreso.Concepto}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                return View("AddForm", ingreso);
            }
        }

        // POST: Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ingreso = await _context.Ingresos.FindAsync(id);

            if (ingreso != null)
            {
                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se eliminó el ingreso: {ingreso.Concepto}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                });

                _context.Ingresos.Remove(ingreso);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var ingreso = await _context.Ingresos
                .Include(i => i.Animal)
                .FirstOrDefaultAsync(i => i.IdIngreso == id);

            if (ingreso == null)
                return NotFound();

            return View(ingreso);
        }

        // GET: Historial
        public async Task<IActionResult> Historial(int idAnimal)
        {
            var historial = await _context.Ingresos
                .Where(i => i.IdAnimal == idAnimal)
                .OrderByDescending(i => i.Fecha)
                .Include(i => i.Animal)
                .ToListAsync();

            var animal = await _context.Animales.FindAsync(idAnimal);

            ViewBag.AnimalNombre = animal?.nombre ?? "Desconocido";

            return View(historial);
        }
    }
}