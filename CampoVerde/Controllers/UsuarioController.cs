using CampoVerde.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class UsuarioController : Controller
    {

        public readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // Al quitar el 'int id', la ruta /Usuario/Perfi funcionará sin parámetros
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            ViewBag.Potreros = await _context.Potreros
                .OrderBy(p => p.Id)
                .ToListAsync();

            ViewBag.ProximaVacuna = await _context.Vacunas
                .Where(v => v.fechaProximaAplicacion >= DateTime.UtcNow.Date)
                .OrderBy(v => v.fechaProximaAplicacion)
                .FirstOrDefaultAsync();

            ViewBag.TotalTareas = await _context.Tareas.CountAsync();

            ViewBag.TareasPendientes = await _context.Tareas
                .CountAsync(t => !t.Completada);

            ViewBag.IngresosHoy = await _context.Ingresos
                .Where(i => i.Fecha.Date == DateTime.UtcNow.Date)
                .SumAsync(i => (decimal?)i.Monto) ?? 0;

            ViewBag.GastosHoy = await _context.Gastos
                .Where(g => g.Fecha.Date == DateTime.UtcNow.Date)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            ViewBag.TotalAnimales = await _context.Animales.CountAsync();

            ViewBag.AlertasVacunas = await _context.Vacunas
                .CountAsync(v => v.fechaProximaAplicacion >= DateTime.UtcNow.Date &&
                                 v.fechaProximaAplicacion <= DateTime.UtcNow.Date.AddDays(7));

            return View();
        }
    }
}