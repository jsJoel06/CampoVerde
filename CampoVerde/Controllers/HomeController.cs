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
            // Carga todos, no solo 4, y pásalos como modelo o ViewBag
            var potreros = await _context.Potreros.OrderBy(p => p.Id).ToListAsync();

            // Si usas ViewBag:
            ViewBag.Potreros = potreros;

            // O si usas un ViewModel (Recomendado):
            // return View(new DashboardViewModel { Potreros = potreros });

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