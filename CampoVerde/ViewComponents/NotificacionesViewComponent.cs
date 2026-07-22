using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampoVerde.Data;
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

            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            var rol = HttpContext.Session.GetString("Rol");


            IQueryable<Notificacion> consulta = _context.Notificaciones;



            // El super administrador ve notificaciones generales
            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta.Where(n => n.ClienteId == clienteId);
            }



            var listaNotificaciones = await consulta
                .OrderByDescending(n => n.Fecha)
                .Take(5)
                .ToListAsync();



            ViewBag.ContadorSinLeer = await consulta
                .CountAsync(n => !n.Leida);



            return View(listaNotificaciones);

        }
    }
}