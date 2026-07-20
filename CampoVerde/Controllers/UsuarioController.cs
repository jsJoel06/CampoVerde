using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            IQueryable<Usuario> usuarios = _context.Usuarios;



            // Si es Super Administrador ve todos
            if (rol != EstadoRol.SUPER_ADMINISTRADOR.ToString())
            {
                usuarios = usuarios.Where(u => u.ClienteId == clienteId);
            }



            return View(await usuarios.ToListAsync());
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != EstadoRol.SUPER_ADMINISTRADOR.ToString() &&
                rol != EstadoRol.ADMINISTRADOR.ToString())
            {
                return Forbid();
            }


            if (rol == EstadoRol.SUPER_ADMINISTRADOR.ToString())
            {
                ViewBag.Roles = new List<EstadoRol>
        {
            EstadoRol.ADMINISTRADOR,
            EstadoRol.VETERINARIO,
            EstadoRol.OPERARIO
        };

                ViewBag.Clientes = new SelectList(
                    _context.Clientes,
                    "Id",
                    "Nombre"
                );
            }
            else
            {
                ViewBag.Roles = new List<EstadoRol>
        {
            EstadoRol.VETERINARIO,
            EstadoRol.OPERARIO
        };
            }


            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (rol != EstadoRol.SUPER_ADMINISTRADOR.ToString() &&
                rol != EstadoRol.ADMINISTRADOR.ToString())
            {
                return Forbid();
            }

            // Si es administrador solo puede crear Veterinarios y Operarios
            if (rol == EstadoRol.ADMINISTRADOR.ToString())
            {
                if (usuario.Rol == EstadoRol.SUPER_ADMINISTRADOR ||
                    usuario.Rol == EstadoRol.ADMINISTRADOR)
                {
                    ModelState.AddModelError("", "No tiene permiso para crear este tipo de usuario.");
                }

                // El cliente siempre será el del administrador
                usuario.ClienteId = clienteId.Value;
            }

            if (ModelState.IsValid)
            {
                // Encriptar contraseña
                usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);

                usuario.estadoActivo = true;
                usuario.FechaRegistro = DateTime.UtcNow;
                usuario.UltimoAcceso = DateTime.UtcNow;

                _context.Usuarios.Add(usuario);

                _context.Notificaciones.Add(new Notificacion
                {
                    Mensaje = $"Se creó el usuario {usuario.nombre}",
                    Fecha = DateTime.UtcNow,
                    Leida = false
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Volver a cargar listas
            if (rol == EstadoRol.SUPER_ADMINISTRADOR.ToString())
            {
                ViewBag.Roles = new List<EstadoRol>
        {
            EstadoRol.ADMINISTRADOR,
            EstadoRol.VETERINARIO,
            EstadoRol.OPERARIO
        };

                ViewBag.Clientes = new SelectList(
                    _context.Clientes,
                    "Id",
                    "Nombre",
                    usuario.ClienteId);
            }
            else
            {
                ViewBag.Roles = new List<EstadoRol>
        {
            EstadoRol.VETERINARIO,
            EstadoRol.OPERARIO
        };
            }

            return View(usuario);
        }
        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();


            ViewBag.Clientes = new SelectList(
                _context.Clientes,
                "Id",
                "NombreEmpresa",
                usuario.ClienteId
            );


            return View(usuario);
        }



        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return NotFound();


            // La contraseña no es obligatoria al editar
            ModelState.Remove("password");


            var usuarioBD = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id);


            if (usuarioBD == null)
                return NotFound();



            var rolActual = HttpContext.Session.GetString("Rol");



            // ADMINISTRADOR no puede cambiar roles administrativos
            if (rolActual == EstadoRol.ADMINISTRADOR.ToString())
            {

                if (usuario.Rol == EstadoRol.SUPER_ADMINISTRADOR ||
                    usuario.Rol == EstadoRol.ADMINISTRADOR)
                {
                    ModelState.AddModelError("Rol",
                        "No tienes permisos para asignar este rol.");
                }


                // Mantener cliente actual
                usuario.ClienteId = usuarioBD.ClienteId;

            }




            if (ModelState.IsValid)
            {

                usuarioBD.nombre = usuario.nombre;

                usuarioBD.email = usuario.email;

                usuarioBD.telefono = usuario.telefono;

                usuarioBD.estadoActivo = usuario.estadoActivo;

                usuarioBD.Rol = usuario.Rol;



                // Solo SUPER_ADMINISTRADOR puede cambiar finca/cliente
                if (rolActual == EstadoRol.SUPER_ADMINISTRADOR.ToString())
                {
                    usuarioBD.ClienteId = usuario.ClienteId;
                }



                // Cambiar contraseña solo si escribe una nueva
                if (!string.IsNullOrWhiteSpace(usuario.password))
                {
                    usuarioBD.password =
                        BCrypt.Net.BCrypt.HashPassword(usuario.password);
                }



                usuarioBD.UltimoAcceso = DateTime.UtcNow;



                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));

            }



            // Mostrar errores para detectar problemas
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }



            // Recargar clientes si falla
            ViewBag.Clientes = new SelectList(
                _context.Clientes,
                "Id",
                "NombreEmpresa",
                usuario.ClienteId
            );


            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 1. ESTE MÉTODO MUESTRA EL FORMULARIO (Responde al clic de "Editar Datos")
        [HttpGet]
        public async Task<IActionResult> EditarPerfil()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Auth");

            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // 2. ESTE MÉTODO PROCESA EL ENVÍO DEL FORMULARIO (Guarda los datos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(Usuario usuario, IFormFile? archivoFoto)
        {
            // Buscar el original en la BD
            var usuarioDb = await _context.Usuarios.FindAsync(usuario.IdUsuario);
            if (usuarioDb == null) return NotFound();

            // Procesar foto si existe
            if (archivoFoto != null && archivoFoto.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + archivoFoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await archivoFoto.CopyToAsync(fileStream);
                }
                usuarioDb.FotoPerfil = uniqueFileName;
            }

            // Actualizar campos permitidos
            usuarioDb.nombre = usuario.nombre;
            usuarioDb.email = usuario.email;

            _context.Update(usuarioDb);
            await _context.SaveChangesAsync();

            // Actualizar sesión
            HttpContext.Session.SetString("Nombre", usuarioDb.nombre);

            // Después de guardar la ruta en la BD:
            HttpContext.Session.SetString("FotoPerfil", usuarioDb.FotoPerfil);

            return RedirectToAction("Perfil");
        }

        // Al quitar el 'int id', la ruta /Usuario/Perfi funcionará sin parámetros
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            // 1. Obtener el ID de la sesión
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // 2. Buscar al usuario en la BD
            var usuario = await _context.Usuarios.FindAsync(idUsuario);

            // 3. Mantener tus ViewBag actuales
            ViewBag.TareasPendientes = await _context.Tareas.CountAsync(t => !t.Completada);
            ViewBag.IngresosHoy = await _context.Ingresos.Where(i => i.Fecha.Date == DateTime.UtcNow.Date).SumAsync(i => (decimal?)i.Monto) ?? 0;
            ViewBag.GastosHoy = await _context.Gastos.Where(g => g.Fecha.Date == DateTime.UtcNow.Date).SumAsync(g => (decimal?)g.Monto) ?? 0;
            ViewBag.AlertasVacunas = await _context.Vacunas.CountAsync(v => v.fechaProximaAplicacion >= DateTime.UtcNow.Date && v.fechaProximaAplicacion <= DateTime.UtcNow.Date.AddDays(7));

            // 4. Pasar el usuario a la vista (¡Esto es lo que faltaba!)
            return View(usuario);
        }
    }
}