using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CampoVerde.Controllers
{
    public class TransaccionAnimalController : Controller
    {

        private readonly AppDbContext _context;


        public TransaccionAnimalController(AppDbContext context)
        {
            _context = context;
        }



        // ==============================
        // LISTADO
        // ==============================

        public async Task<IActionResult> Index()
        {

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            IQueryable<TransaccionAnimal> consulta =
                _context.TransaccionesAnimales
                .Include(t => t.Animal)
                .Include(t => t.Cliente);



            if (rol != "SUPER_ADMINISTRADOR")
            {
                consulta = consulta
                    .Where(t => t.ClienteId == clienteId);
            }



            var transacciones =
                await consulta
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();



            return View(transacciones);

        }




        // ==============================
        // CREAR GET
        // ==============================

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            ViewBag.Animales =
                new SelectList(
                    await _context.Animales
                    .Where(a => a.ClienteId == clienteId)
                    .ToListAsync(),
                    "IdAnimal",
                    "nombre"
                );


            return View();

        }





        // ==============================
        // CREAR POST
        // ==============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransaccionAnimal transaccion)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
            {
                return Unauthorized();
            }


            try
            {

                transaccion.ClienteId = clienteId;
                transaccion.Fecha = DateTime.UtcNow;



                // ============================
                // COMPRA DE ANIMAL
                // ============================

                if (transaccion.Tipo == TipoTransaccion.Compra)
                {


                    if (string.IsNullOrEmpty(transaccion.NombreAnimal))
                    {
                        ModelState.AddModelError("",
                        "Debe colocar el nombre del animal comprado.");

                        return View(transaccion);
                    }



                    // Crear animal nuevo

                    var nuevoAnimal = new Animal
                    {

                        nombre = transaccion.NombreAnimal,

                        raza = transaccion.Raza,

                        pesoActual = (double)(transaccion.Peso ?? 0),

                        fechaNacimiento = transaccion.FechaNacimiento,


                        Estado = EstadoAnimal.ACTIVO,


                        ClienteId = clienteId.Value

                    };



                    _context.Animales.Add(nuevoAnimal);



                    // Guardamos primero para obtener el IdAnimal

                    await _context.SaveChangesAsync();



                    // Relacionamos la compra con el animal creado

                    transaccion.IdAnimal = nuevoAnimal.IdAnimal;



                }



                // ============================
                // VENTA DE ANIMAL
                // ============================

                else if (transaccion.Tipo == TipoTransaccion.Venta)
                {


                    if (transaccion.IdAnimal == null)
                    {
                        ModelState.AddModelError("",
                        "Debe seleccionar un animal.");

                        return View(transaccion);
                    }



                    var animal = await _context.Animales
                        .FirstOrDefaultAsync(a =>
                            a.IdAnimal == transaccion.IdAnimal &&
                            a.ClienteId == clienteId);



                    if (animal == null)
                    {
                        ModelState.AddModelError("",
                        "El animal no existe.");

                        return View(transaccion);
                    }




                    animal.Estado = EstadoAnimal.VENDIDO;


                    _context.Animales.Update(animal);


                }



                // ============================
                // GUARDAR TRANSACCIÓN
                // ============================


                _context.TransaccionesAnimales.Add(transaccion);


                await _context.SaveChangesAsync();



                // Notificación

                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = transaccion.Tipo == TipoTransaccion.Compra
                    ?
                    $"Se registró la compra del animal {transaccion.NombreAnimal}"
                    :
                    $"Se vendió el animal correctamente.",

                    Fecha = DateTime.UtcNow,

                    Leida = false

                });


                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

                ModelState.AddModelError("",
                "Error al guardar la transacción.");

            }



            return View(transaccion);
        }




        // ==============================
        // DETAILS
        // ==============================

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
                return NotFound();



            var transaccion =
                await _context.TransaccionesAnimales
                .Include(t => t.Animal)
                .Include(t => t.Cliente)
                .FirstOrDefaultAsync(t =>
                    t.IdTransaccion == id);



            if (transaccion == null)
                return NotFound();



            return View(transaccion);

        }





        // ==============================
        // EDIT GET
        // ==============================

        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
                return NotFound();



            var transaccion =
                await _context.TransaccionesAnimales
                .FindAsync(id);



            if (transaccion == null)
                return NotFound();



            ViewBag.Animales =
                new SelectList(
                    _context.Animales,
                    "IdAnimal",
                    "nombre",
                    transaccion.IdAnimal
                );



            return View(transaccion);

        }





        // ==============================
        // EDIT POST
        // ==============================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            TransaccionAnimal transaccion)
        {


            if (id != transaccion.IdTransaccion)
                return NotFound();



            if (ModelState.IsValid)
            {

                var existente =
                    await _context.TransaccionesAnimales
                    .FindAsync(id);



                if (existente == null)
                    return NotFound();



                existente.IdAnimal =
                    transaccion.IdAnimal;


                existente.Tipo =
                    transaccion.Tipo;


                existente.Monto =
                    transaccion.Monto;


                existente.Tercero =
                    transaccion.Tercero;


                existente.Notas =
                    transaccion.Notas;



                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));

            }



            return View(transaccion);

        }





        // ==============================
        // DELETE GET
        // ==============================

        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
                return NotFound();



            var transaccion =
                await _context.TransaccionesAnimales
                .Include(t => t.Animal)
                .FirstOrDefaultAsync(t =>
                    t.IdTransaccion == id);



            if (transaccion == null)
                return NotFound();



            return View(transaccion);

        }





        // ==============================
        // DELETE POST
        // ==============================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var transaccion =
                await _context.TransaccionesAnimales
                .FindAsync(id);



            if (transaccion != null)
            {

                _context.TransaccionesAnimales
                    .Remove(transaccion);


                await _context.SaveChangesAsync();

            }



            return RedirectToAction(nameof(Index));

        }

    }
}