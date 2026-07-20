using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class ResumenFinancieroController : Controller
    {
        private readonly AppDbContext _context;


        public ResumenFinancieroController(AppDbContext context)
        {
            _context = context;
        }



        // =====================================
        // LISTAR RESUMENES
        // =====================================

        public async Task<IActionResult> Index()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            var rol = HttpContext.Session.GetString("Rol");


            IQueryable<ResumenFinanciero> resumenes =
                _context.ResumenesFinancieros
                .Include(r => r.Cliente);



            // Filtrar por cliente si no es administrador

            if (rol != "SUPER_ADMINISTRADOR")
            {
                resumenes = resumenes
                    .Where(r => r.ClienteId == clienteId);
            }



            return View(
                await resumenes
                .OrderByDescending(r => r.Año)
                .ThenByDescending(r => r.Mes)
                .ToListAsync()
            );
        }





        // =====================================
        // GENERAR RESUMEN MENSUAL
        // =====================================


        [HttpPost]
        public async Task<IActionResult> GenerarResumen()
        {

            var clienteId =
                HttpContext.Session.GetInt32("ClienteId");


            if (clienteId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }



            var fechaActual = DateTime.UtcNow;


            int mes = fechaActual.Month;
            int año = fechaActual.Year;



            // ==============================
            // INGRESOS DEL MES
            // ==============================


            var totalIngresos = await _context.Ingresos

                .Where(i =>
                    i.Animal.ClienteId == clienteId &&

                    i.Fecha.Month == mes &&

                    i.Fecha.Year == año
                )

                .SumAsync(i => (decimal?)i.Monto) ?? 0;





            // ==============================
            // GASTOS DEL MES
            // ==============================


            var totalGastos = await _context.Gastos

                .Where(g =>
                    g.Animal.ClienteId == clienteId &&

                    g.Fecha.Month == mes &&

                    g.Fecha.Year == año
                )

                .SumAsync(g => (decimal?)g.Monto) ?? 0;






            // ==============================
            // VERIFICAR SI EXISTE
            // ==============================


            var resumenExistente =
                await _context.ResumenesFinancieros

                .FirstOrDefaultAsync(r =>

                    r.ClienteId == clienteId &&

                    r.Mes == mes &&

                    r.Año == año
                );





            if (resumenExistente != null)
            {

                resumenExistente.TotalIngresos = totalIngresos;

                resumenExistente.TotalGastos = totalGastos;

                resumenExistente.Ganancia =
                    totalIngresos - totalGastos;


                resumenExistente.FechaGeneracion =
                    DateTime.UtcNow;



                _context.Update(resumenExistente);

            }
            else
            {

                var nuevoResumen = new ResumenFinanciero
                {

                    ClienteId = clienteId,


                    Mes = mes,


                    Año = año,


                    TotalIngresos = totalIngresos,


                    TotalGastos = totalGastos,


                    Ganancia =
                        totalIngresos - totalGastos,


                    FechaGeneracion =
                        DateTime.UtcNow

                };


                _context.ResumenesFinancieros
                    .Add(nuevoResumen);

            }





            await _context.SaveChangesAsync();



            TempData["Mensaje"] =
                "Resumen financiero generado correctamente";



            return RedirectToAction(nameof(Index));

        }







        // =====================================
        // DETALLES
        // =====================================


        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return NotFound();



            var resumen =
                await _context.ResumenesFinancieros

                .Include(r => r.Cliente)

                .FirstOrDefaultAsync(r =>
                    r.IdResumen == id);



            if (resumen == null)
                return NotFound();



            return View(resumen);

        }








        // =====================================
        // ELIMINAR
        // =====================================


        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
                return NotFound();



            var resumen =
                await _context.ResumenesFinancieros

                .Include(r => r.Cliente)

                .FirstOrDefaultAsync(r =>
                    r.IdResumen == id);



            if (resumen == null)
                return NotFound();



            return View(resumen);

        }






        [HttpPost, ActionName("Delete")]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var resumen =
                await _context.ResumenesFinancieros
                .FindAsync(id);



            if (resumen != null)
            {

                _context.ResumenesFinancieros
                    .Remove(resumen);


                await _context.SaveChangesAsync();

            }



            return RedirectToAction(nameof(Index));

        }

    }
}