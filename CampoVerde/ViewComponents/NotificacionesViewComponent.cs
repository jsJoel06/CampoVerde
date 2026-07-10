using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampoVerde.Data; // Ajusta según tu namespace del DbContext
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CampoVerde.Models;

namespace CampoVerde.ViewComponents
{
    public class NotificacionesViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public NotificacionesViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Obtenemos las últimas 5 notificaciones sin leer o recientes
            var listaNotificaciones = await _context.Notificaciones
                .OrderByDescending(n => n.Fecha)
                .Take(5)
                .ToListAsync();

            // Contamos cuántas están sin leer para activar el punto verde
            ViewBag.ContadorSinLeer = await _context.Notificaciones.CountAsync(n => !n.Leida);

            return View(listaNotificaciones);
        }
    }
}