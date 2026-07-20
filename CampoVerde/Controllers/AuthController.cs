using CampoVerde.Data;
using CampoVerde.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampoVerde.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            // 1. Buscar usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.email == email);


            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.password))
            {
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View();
            }


            if (!usuario.estadoActivo)
            {
                ViewBag.Error = "El usuario está desactivado.";
                return View();
            }



            // 2. Registrar acceso

            usuario.UltimoAcceso = DateTime.UtcNow;

            await _context.SaveChangesAsync();




            // 3. Crear sesión

            HttpContext.Session.SetInt32(
                "IdUsuario",
                usuario.IdUsuario
            );


            HttpContext.Session.SetString(
                "Nombre",
                usuario.nombre ?? "Usuario"
            );


            HttpContext.Session.SetString(
                "Rol",
                usuario.Rol.ToString()
            );


            // NUEVO: Guardar la empresa/finca del usuario
            // Guardar el ClienteId en sesión
            if (usuario.Rol == EstadoRol.SUPER_ADMINISTRADOR)
            {
                // El Super Administrador no pertenece a ningún cliente
                HttpContext.Session.SetInt32("ClienteId", 0);
            }
            else
            {
                if (!usuario.ClienteId.HasValue)
                {
                    ViewBag.Error = "Este usuario no tiene un cliente asignado.";
                    return View();
                }

                HttpContext.Session.SetInt32("ClienteId", usuario.ClienteId.Value);
            }



            HttpContext.Session.SetString(
                "FotoPerfil",
                usuario.FotoPerfil ?? ""
            );



            return RedirectToAction("Index", "Home");
        }

        // ==========================
        // GET: Register
        // ==========================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ==========================
        // POST: Register
        // ==========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            // Verificar si el correo ya existe
            bool existe = await _context.Usuarios
                .AnyAsync(u => u.email == usuario.email);

            if (existe)
            {
                ViewBag.Error = "Ya existe una cuenta registrada con ese correo.";
                return View(usuario);
            }

            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);

            // Valores por defecto
            usuario.estadoActivo = true;
            usuario.FechaRegistro = DateTime.UtcNow;
            usuario.UltimoAcceso = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Usuario registrado correctamente.";

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }


}