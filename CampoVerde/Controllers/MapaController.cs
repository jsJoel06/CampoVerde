using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class MapaController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public MapaController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // DASHBOARD
    public async Task<IActionResult> Index()
    {
        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        IQueryable<Potrero> consulta = _context.Potreros;

        if (rol != "SUPER_ADMINISTRADOR")
        {
            consulta = consulta.Where(p => p.ClienteId == clienteId);
        }

        return View(await consulta.ToListAsync());
    }


    // FORM CREATE/EDIT
    public async Task<IActionResult> AddForm(int? id)
    {
        var rol = HttpContext.Session.GetString("Rol");
        var clienteId = HttpContext.Session.GetInt32("ClienteId");

        if (id == null)
            return View(new Potrero());

        IQueryable<Potrero> consulta = _context.Potreros;

        if (rol != "SUPER_ADMINISTRADOR")
        {
            consulta = consulta.Where(p => p.ClienteId == clienteId);
        }

        var potrero = await consulta.FirstOrDefaultAsync(p => p.Id == id);

        if (potrero == null)
        {
            potrero = new Potrero
            {
                Id = id.Value,
                Nombre = $"Lote {id.Value}",
                RutasFotos = ""
            };
        }

        return View(potrero);
    }

    // GUARDAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(Potrero potrero)
    {
        if (!ModelState.IsValid)
            return View("AddForm", potrero);

        var clienteId = HttpContext.Session.GetInt32("ClienteId");
        var rol = HttpContext.Session.GetString("Rol");

        if (clienteId == null)
            return Unauthorized();

        var carpeta = Path.Combine(_env.WebRootPath, "uploads");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        var fotosNuevas = new List<string>();

        if (potrero.ArchivosFotos != null && potrero.ArchivosFotos.Any())
        {
            foreach (var foto in potrero.ArchivosFotos)
            {
                var nombre = Guid.NewGuid() + Path.GetExtension(foto.FileName);
                var ruta = Path.Combine(carpeta, nombre);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                fotosNuevas.Add(nombre);
            }
        }

        IQueryable<Potrero> consulta = _context.Potreros;

        if (rol != "SUPER_ADMINISTRADOR")
        {
            consulta = consulta.Where(p => p.ClienteId == clienteId);
        }

        var existente = await consulta.FirstOrDefaultAsync(x => x.Id == potrero.Id);

        if (existente == null)
        {
            existente = new Potrero
            {
                Id = potrero.Id,
                Nombre = potrero.Nombre,
                RutasFotos = string.Join(";", fotosNuevas),
                ClienteId = clienteId.Value
            };

            _context.Potreros.Add(existente);

            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se registró un nuevo potrero: {existente.Nombre}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });
        }
        else
        {
            existente.Nombre = potrero.Nombre;

            var fotosAntiguas = string.IsNullOrEmpty(existente.RutasFotos)
                ? new List<string>()
                : existente.RutasFotos.Split(';').ToList();

            fotosAntiguas.AddRange(fotosNuevas);

            existente.RutasFotos = string.Join(";", fotosAntiguas);

            _context.Potreros.Update(existente);

            _context.Notificaciones.Add(new Notificacion
            {
                Mensaje = $"Se actualizó el potrero: {existente.Nombre}",
                Fecha = DateTime.UtcNow,
                Leida = false
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}