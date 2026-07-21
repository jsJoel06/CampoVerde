using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class LicenciaController : Controller
    {
        private readonly AppDbContext _context;

        public LicenciaController(AppDbContext context)
        {
            _context = context;
        }


        //=========================
        // INDEX
        //=========================
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();


            var licencias = await _context.Licencias
                .Include(l => l.Cliente)
                .OrderByDescending(l => l.FechaVencimiento)
                .ToListAsync();


            return View(licencias);
        }



        //=========================
        // DETAILS
        //=========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();


            var licencia = await _context.Licencias
                .Include(l => l.Cliente)
                .FirstOrDefaultAsync(l => l.IdLicencia == id);


            if (licencia == null)
                return NotFound();


            return View(licencia);
        }



        //=========================
        // CREATE GET
        //=========================
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();


            CargarClientes();


            return View();
        }



        //=========================
        // CREATE POST
        //=========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Licencia licencia)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();


            if (ModelState.IsValid)
            {

                // Verificar si el cliente ya tiene licencia activa
                var existe = await _context.Licencias
                    .AnyAsync(l =>
                        l.ClienteId == licencia.ClienteId &&
                        l.Activa);


                if (existe)
                {
                    ModelState.AddModelError("",
                        "Este cliente ya tiene una licencia activa.");

                    CargarClientes(licencia.ClienteId);

                    return View(licencia);
                }



                licencia.FechaInicio = DateTime.UtcNow;

                licencia.Activa = true;


                _context.Licencias.Add(licencia);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            CargarClientes(licencia.ClienteId);

            return View(licencia);
        }



        //=========================
        // EDIT GET
        //=========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();


            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();



            var licencia = await _context.Licencias
                .FindAsync(id);


            if (licencia == null)
                return NotFound();


            CargarClientes(licencia.ClienteId);


            return View(licencia);
        }



        //=========================
        // EDIT POST
        //=========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Licencia licencia)
        {

            if (id != licencia.IdLicencia)
                return NotFound();


            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();



            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(licencia);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Licencias
                        .Any(l => l.IdLicencia == id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }


            CargarClientes(licencia.ClienteId);

            return View(licencia);
        }




        //=========================
        // DELETE GET
        //=========================
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
                return NotFound();


            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();



            var licencia = await _context.Licencias
                .Include(l => l.Cliente)
                .FirstOrDefaultAsync(l => l.IdLicencia == id);



            if (licencia == null)
                return NotFound();



            return View(licencia);
        }




        //=========================
        // DELETE POST
        //=========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "SUPER_ADMINISTRADOR")
                return Unauthorized();



            var licencia = await _context.Licencias
                .FindAsync(id);



            if (licencia == null)
                return NotFound();



            _context.Licencias.Remove(licencia);


            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }





        //=========================
        // MÉTODO AUXILIAR
        //=========================
        private void CargarClientes(int? seleccionado = null)
        {
            ViewData["ClienteId"] = new SelectList(
                _context.Clientes,
                "Id",
                "Nombre",
                seleccionado
            );
        }

    }
}