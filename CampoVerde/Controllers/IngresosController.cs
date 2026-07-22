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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            // SUPER ADMINISTRADOR no ve ingresos de clientes
            if (rol == "SUPER_ADMINISTRADOR")
            {
                return View(new List<Ingreso>());
            }



            var ingresos = await _context.Ingresos
                .Include(i => i.Animal)
                .Where(i => i.ClienteId == clienteId)
                .ToListAsync();


            return View(ingresos);
        }

        // GET: Ingresos/AddForm
        public IActionResult AddForm(int? id)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Animal> animales = _context.Animales.OrderBy(a => a.nombre);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId)
                                   .OrderBy(a => a.nombre);
            }

            ViewBag.Animales = new SelectList(animales.ToList(), "IdAnimal", "nombre");

            if (id == null)
            {
                return View(new Ingreso
                {
                    Fecha = DateTime.UtcNow
                });
            }

            IQueryable<Ingreso> consulta = _context.Ingresos;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(i => i.ClienteId == clienteId);
            }

            var ingreso = consulta.FirstOrDefault(i => i.IdIngreso == id);

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

            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var rol = HttpContext.Session.GetString("Rol");

            if (clienteId == null)
                return Unauthorized();

            IQueryable<Animal> animales = _context.Animales;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                animales = animales.Where(a => a.ClienteId == clienteId);
            }

            ViewBag.Animales = new SelectList(
                animales.ToList(),
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
                    // Asignar el cliente del usuario logueado
                    ingreso.ClienteId = clienteId.Value;

                    _context.Ingresos.Add(ingreso);

                    _context.Notificaciones.Add(new Notificacion
                    {
                        ClienteId = clienteId.Value,
                        Mensaje = $"Se registró un nuevo ingreso: {ingreso.Concepto}",
                        Fecha = DateTime.UtcNow,
                        Leida = false
                    });
                }
                else
                {
                    IQueryable<Ingreso> consulta = _context.Ingresos;

                    if (rol != "SUPER_ADMINISTRADOR")
                    {
                        consulta = consulta.Where(i => i.ClienteId == clienteId);
                    }

                    var ingresoExistente = await consulta
                        .FirstOrDefaultAsync(i => i.IdIngreso == ingreso.IdIngreso);

                    if (ingresoExistente == null)
                        return NotFound();

                    ingresoExistente.Monto = ingreso.Monto;
                    ingresoExistente.Concepto = ingreso.Concepto;
                    ingresoExistente.Fecha = ingreso.Fecha;
                    ingresoExistente.Notas = ingreso.Notas;
                    ingresoExistente.IdAnimal = ingreso.IdAnimal;

                    // Mantener el ClienteId original
                    ingresoExistente.ClienteId = ingresoExistente.ClienteId;

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
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Ingreso> consulta = _context.Ingresos;

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(i => i.ClienteId == clienteId);
            }

            var ingreso = await consulta.FirstOrDefaultAsync(i => i.IdIngreso == id);

            if (ingreso == null)
                return NotFound();

            _context.Notificaciones.Add(new Notificacion
            {
                ClienteId = clienteId.Value,
                Mensaje = $"Se eliminó el ingreso: {ingreso.Concepto}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });

            _context.Ingresos.Remove(ingreso);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Ingreso> consulta = _context.Ingresos
                .Include(i => i.Animal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(i => i.ClienteId == clienteId);
            }

            var ingreso = await consulta.FirstOrDefaultAsync(i => i.IdIngreso == id);

            if (ingreso == null)
                return NotFound();

            return View(ingreso);
        }


        // GET: Historial
        public async Task<IActionResult> Historial(int idAnimal)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            IQueryable<Ingreso> consulta = _context.Ingresos
                .Include(i => i.Animal)
                .Where(i => i.IdAnimal == idAnimal);

            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(i => i.ClienteId == clienteId);
            }

            var historial = await consulta
                .OrderByDescending(i => i.Fecha)
                .ToListAsync();

            var animal = await _context.Animales
                .FirstOrDefaultAsync(a => a.IdAnimal == idAnimal &&
                    (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId));

            ViewBag.AnimalNombre = animal?.nombre ?? "Desconocido";

            return View(historial);
        }
    }
}