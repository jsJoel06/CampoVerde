using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CampoVerde.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // En HomeController.cs
        public async Task<IActionResult> Index()
        {

            ViewBag.Prueba = "Hola Mundo";

            // Potreros
            ViewBag.Potreros = await _context.Potreros
                .OrderBy(p => p.Id)
                .ToListAsync();

            // Próxima vacuna
            ViewBag.ProximaVacuna = await _context.Vacunas
                .Where(v => v.fechaProximaAplicacion.Date >= DateTime.UtcNow.Date)
                .OrderBy(v => v.fechaProximaAplicacion)
                .FirstOrDefaultAsync();

            // Total de tareas
            ViewBag.TotalTareas = await _context.Tareas.CountAsync();

            // Tareas pendientes
            ViewBag.TareasPendientes = await _context.Tareas
                .CountAsync(t => !t.Completada);

            // Ingresos de hoy
            ViewBag.IngresosHoy = await _context.Ingresos
                .Where(i => i.Fecha.Date == DateTime.UtcNow.Date)
                .SumAsync(i => (decimal?)i.Monto) ?? 0;

            // Gastos de hoy
            ViewBag.GastosHoy = await _context.Gastos
                .Where(g => g.Fecha.Date == DateTime.UtcNow.Date)
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            // Total de animales
            ViewBag.TotalAnimales = await _context.Animales.CountAsync();

            // Vacunas próximas (7 días)
            ViewBag.AlertasVacunas = await _context.Vacunas
                .CountAsync(v => v.fechaProximaAplicacion >= DateTime.UtcNow.Date &&
                                 v.fechaProximaAplicacion <= DateTime.UtcNow.Date.AddDays(7));

            return View();
        }

        // Dashboard funcional con corrección de fechas
        public async Task<IActionResult> Dashboard()
        {
            // Obtenemos la fecha actual en formato UTC para evitar conflictos de zona horaria con PostgreSQL
            var hoy = DateTime.UtcNow;

            ViewBag.TotalAnimales = await _context.Animales.CountAsync();

            ViewBag.TareasPendientes = await _context.Tareas
                .CountAsync();

            
            ViewBag.VacunasVencidas = await _context.Vacunas
                .CountAsync(v => v.fechaProximaAplicacion < hoy);

            ViewBag.IngresosMes = await _context.Ingresos
                .Where(i => i.Fecha.Month == hoy.Month && i.Fecha.Year == hoy.Year)
                .SumAsync(i => (decimal?)i.Monto) ?? 0; // Manejo de nulos por si no hay ingresos

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}