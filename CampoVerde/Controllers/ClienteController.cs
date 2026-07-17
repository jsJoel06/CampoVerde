using CampoVerde.Data;
using CampoVerde.Models;
using CampoVerde.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    [AuthorizeRole(Permisos.SuperAdministrador)]
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;


        public ClienteController(AppDbContext context)
        {
            _context = context;
        }



        // GET: Cliente
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Usuarios)
                .ToListAsync();

            return View(clientes);
        }




        // GET: Cliente/Create
        public IActionResult Create()
        {
            return View();
        }





        // POST: Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {

                // Fecha de registro en UTC
                cliente.FechaRegistro = DateTime.UtcNow;


                // Convertir fecha de vencimiento a UTC
                if (cliente.FechaVencimiento.HasValue)
                {
                    cliente.FechaVencimiento = DateTime.SpecifyKind(
                        cliente.FechaVencimiento.Value,
                        DateTimeKind.Utc
                    );
                }


                _context.Add(cliente);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }


            return View(cliente);
        }


        // GET: Cliente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }


            var cliente = await _context.Clientes
                .FindAsync(id);


            if (cliente == null)
            {
                return NotFound();
            }


            return View(cliente);
        }






        // POST: Cliente/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {

            if (id != cliente.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {

                try
                {

                    _context.Update(cliente);

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!ClienteExiste(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }


                return RedirectToAction(nameof(Index));

            }


            return View(cliente);
        }





        // GET: Cliente/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }


            var cliente = await _context.Clientes
                .Include(c => c.Usuarios)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (cliente == null)
            {
                return NotFound();
            }


            return View(cliente);
        }





        // POST: Cliente/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var cliente = await _context.Clientes.FindAsync(id);


            if (cliente != null)
            {

                _context.Clientes.Remove(cliente);

                await _context.SaveChangesAsync();

            }


            return RedirectToAction(nameof(Index));
        }





        private bool ClienteExiste(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

    }
}