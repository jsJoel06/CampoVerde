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

        // HOME
        public async Task<IActionResult> Index()
        {
            ViewBag.Prueba = "Hola Mundo";


            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            Console.WriteLine("ROL: " + rol);
            Console.WriteLine("CLIENTE SESION: " + clienteId);


            IQueryable<Animal> animales = _context.Animales;

            IQueryable<Tarea> tareas = _context.Tareas
                .Include(t => t.Animal);

            IQueryable<Ingreso> ingresos = _context.Ingresos
                .Include(i => i.Animal);

            IQueryable<Gasto> gastos = _context.Gastos
                .Include(g => g.Animal);

            IQueryable<Vacuna> vacunas = _context.Vacunas
                .Include(v => v.Animal);

            IQueryable<Potrero> potreros = _context.Potreros;

            IQueryable<Produccion> producciones = _context.Producciones
          .Include(p => p.Animal);




        

                animales = animales
                    .Where(a => a.ClienteId == clienteId);


                tareas = tareas
                    .Where(t => t.Animal.ClienteId == clienteId);


                ingresos = ingresos
                    .Where(i => i.Animal.ClienteId == clienteId);


                gastos = gastos
                    .Where(g => g.Animal.ClienteId == clienteId);


                vacunas = vacunas
                    .Where(v => v.Animal.ClienteId == clienteId);



                potreros = potreros
                    .Where(p => p.ClienteId == clienteId);

                producciones = producciones
                       .Where(p => p.ClienteId == clienteId);

            


            // ==============================
            // PRÓXIMOS PARTOS
            // ==============================

            var animalesEmbarazados = await _context.Animales
          .Where(a =>
        a.Estado == EstadoAnimal.EMBARAZADA &&
        a.FechaEmbarazo != null &&
        (rol == "SUPER_ADMINISTRADOR" || a.ClienteId == clienteId)
    )
    .ToListAsync();

           

            var proximosPartos = animalesEmbarazados
                .Select(a => new
                    {
                       Animal = a,
                       FechaParto = a.FechaEmbarazo!.Value.AddDays(283),
                       DiasRestantes = (a.FechaEmbarazo.Value.AddDays(283) - DateTime.UtcNow).Days
                    })
                   .OrderBy(x => x.FechaParto)
                   .ToList();

            ViewBag.ListaPartos = proximosPartos;

            ViewBag.TotalEmbarazadas = animalesEmbarazados.Count();
            ViewBag.TotalPartos = proximosPartos.Count();

            // =========================
            // POTREROS
            // =========================

            ViewBag.Potreros = await potreros
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();





            // =========================
            // VACUNAS
            // =========================


            ViewBag.ProximaVacuna = await vacunas
                .OrderBy(v => v.fechaProximaAplicacion)
                .FirstOrDefaultAsync();



            ViewBag.VacunasPendientes = await vacunas
                .CountAsync(v =>
                    v.fechaProximaAplicacion >= DateTime.UtcNow.Date);






            // =========================
            // ANIMALES
            // =========================


            ViewBag.TotalReses =
                await animales.CountAsync();



            ViewBag.AnimalesActivos =
                await animales
                .CountAsync(a =>
                    a.Estado == EstadoAnimal.ACTIVO);



            ViewBag.AnimalesEnfermos =
                await animales
                .CountAsync(a =>
                    a.Estado == EstadoAnimal.ENFERMO);



            ViewBag.AnimalesObservacion =
                await animales
                .CountAsync(a =>
                    a.Estado == EstadoAnimal.OBSERVACION);



            // =========================
            // TAREAS
            // =========================


            ViewBag.TotalTareas =
                await tareas.CountAsync();


            ViewBag.TareasPendientes =
                await tareas
                .CountAsync(t =>
                    !t.Completada);


            // =========================
            // FINANZAS
            // =========================


            ViewBag.IngresosHoy =
                await ingresos
                .Where(i =>
                    i.Fecha.Date == DateTime.UtcNow.Date)
                .SumAsync(i =>
                    (decimal?)i.Monto) ?? 0;



            ViewBag.GastosHoy =
                await gastos
                .Where(g =>
                    g.Fecha.Date == DateTime.UtcNow.Date)
                .SumAsync(g =>
                    (decimal?)g.Monto) ?? 0;


            // =========================
            // STOCK BAJO
            // =========================
            // Ajustar cuando tengas modelo de inventario


            ViewBag.StockBajo = 0;


            // =========================
            // ULTIMOS ANIMALES
            // =========================


            ViewBag.UltimosAnimales =
                await animales
                .OrderByDescending(a => a.IdAnimal)
                .Take(5)
                .ToListAsync();


            // =========================
            // PARTOS REGISTRADOS + EMBARAZOS
            // =========================


            // Partos registrados manualmente
            var listaPartosRegistrados = await _context.Partos
                .Include(p => p.Animal)
                .Where(p => p.FechaParto.Date >= DateTime.UtcNow.Date)
                .OrderBy(p => p.FechaParto)
                .Take(5)
                .ToListAsync();


            // Mantener los partos calculados por embarazo
            ViewBag.ListaPartos = proximosPartos;


            ViewBag.ProximosPartos = proximosPartos.Count;

            Console.WriteLine("Partos encontrados: " + proximosPartos.Count);

            foreach (var p in proximosPartos)
            {
                Console.WriteLine(
                    $"Animal: {p.Animal.nombre} - Fecha: {p.FechaParto} - Dias: {p.DiasRestantes}"
                );
            }

            // =========================
            // PRODUCCIÓN DE LECHE (ÚLTIMOS 7 DÍAS)
            // =========================



            var hoy = DateTime.UtcNow.Date;
            var inicio = hoy.AddDays(-6);

            var registrosProduccion = await producciones
                .Where(p => p.fechaProduccion >= inicio &&
                            p.fechaProduccion < hoy.AddDays(1))
                .ToListAsync();

            var labels = new List<string>();
            var litros = new List<double>();

            for (int i = 0; i < 7; i++)
            {
                var fecha = inicio.AddDays(i);

                labels.Add(fecha.ToString("ddd"));

                litros.Add(
                    registrosProduccion
                        .Where(p => p.fechaProduccion.Date == fecha)
                        .Sum(p => p.cantidadLeche)
                );
            }

            ViewBag.Labels = labels;
            ViewBag.Litros = litros;

            // Producción total de hoy
            ViewBag.ProduccionHoy = registrosProduccion
                .Where(p => p.fechaProduccion.Date == hoy)
                .Sum(p => p.cantidadLeche);

            // Producción total de la semana
            ViewBag.ProduccionSemana = registrosProduccion
                .Sum(p => p.cantidadLeche);


            return View();
        }

        // DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            var hoy = DateTime.UtcNow;
            var inicioMes = new DateTime(
    hoy.Year,
    hoy.Month,
    1,
    0,
    0,
    0,
    DateTimeKind.Utc
);

            var rol = HttpContext.Session.GetString("Rol");
            var clienteId = HttpContext.Session.GetInt32("ClienteId");


            // ==============================
            // CONSULTAS BASE
            // ==============================

            IQueryable<Animal> animales = _context.Animales;

            IQueryable<Tarea> tareas =
                _context.Tareas
                .Include(t => t.Animal);


            IQueryable<Vacuna> vacunas =
                _context.Vacunas
                .Include(v => v.Animal);


            IQueryable<Ingreso> ingresos =
                _context.Ingresos
                .Include(i => i.Animal);


            IQueryable<Gasto> gastos =
                _context.Gastos
                .Include(g => g.Animal);


            IQueryable<Produccion> producciones =
                _context.Producciones
                .Include(p => p.Animal);


            IQueryable<Parto> partos =
                _context.Partos
                .Include(p => p.Animal);

            if (rol == "SUPER_ADMINISTRADOR")
            {
                ViewBag.TotalClientes = await _context.Clientes.CountAsync();
                ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
                ViewBag.LicenciasActivas = await _context.Licencias.CountAsync(l => l.Activa);

                ViewBag.ClientesActivos = await _context.Clientes.CountAsync(c => c.Activo);
                ViewBag.ClientesVencidos = await _context.Clientes.CountAsync(c => !c.Activo);

                return View("DashboardAdmin");
            }
            // ==============================
            // FILTRO POR CLIENTE
            // ==============================


            animales =
                    animales.Where(a =>
                    a.ClienteId == clienteId);


                tareas =
                    tareas.Where(t =>
                    t.Animal.ClienteId == clienteId);


                vacunas =
                    vacunas.Where(v =>
                    v.Animal.ClienteId == clienteId);


                ingresos =
                    ingresos.Where(i =>
                    i.Animal.ClienteId == clienteId);


                gastos =
                    gastos.Where(g =>
                    g.ClienteId == clienteId);


                producciones =
                    producciones.Where(p =>
                    p.Animal.ClienteId == clienteId);


                partos =
                    partos.Where(p =>
                    p.Animal.ClienteId == clienteId);

            


            // =========================
            // RESUMEN FINANCIERO
            // =========================

            var fechaActual = DateTime.UtcNow;

            var clienteIdSesion = HttpContext.Session.GetInt32("ClienteId");


            if (clienteIdSesion == null)
            {
                return Unauthorized();
            }


            // INGRESOS DEL MES

            var ingresosMes = await _context.Ingresos
                .Where(i =>
                    i.ClienteId == clienteIdSesion &&
                    i.Fecha.Month == fechaActual.Month &&
                    i.Fecha.Year == fechaActual.Year
                )
                .SumAsync(i => (decimal?)i.Monto) ?? 0;



            // GASTOS DEL MES

            var gastosMes = await _context.Gastos
                .Where(g =>
                    g.ClienteId == clienteIdSesion &&
                    g.Fecha.Month == fechaActual.Month &&
                    g.Fecha.Year == fechaActual.Year
                )
                .SumAsync(g => (decimal?)g.Monto) ?? 0;



            ViewBag.IngresosMes = ingresosMes;

            ViewBag.GastosMes = gastosMes;

            ViewBag.GananciaMes = ingresosMes - gastosMes;



            // ==============================
            // TARJETAS PRINCIPALES
            // ==============================


            ViewBag.TotalAnimales =
                await animales.CountAsync();



            ViewBag.TareasPendientes =
                await tareas.CountAsync(t =>
                !t.Completada);



            ViewBag.VacunasVencidas =
                await vacunas.CountAsync(v =>
                v.fechaProximaAplicacion < hoy);



            ViewBag.IngresosMes =
                await ingresos
                .Where(i =>
                i.Fecha >= inicioMes)
                .SumAsync(i =>
                (decimal?)i.Monto) ?? 0;



            // ==============================
            // GASTOS
            // ==============================


            ViewBag.GastosMes =
                await gastos
                .Where(g =>
                g.Fecha >= inicioMes)
                .SumAsync(g =>
                (decimal?)g.Monto) ?? 0;



            // ==============================
            // PRODUCCIÓN
            // ==============================


            ViewBag.ProduccionHoy =
                await producciones
                .Where(p =>
                p.fechaProduccion.Date == hoy.Date)
                .SumAsync(p =>
                (decimal?)p.cantidadLeche) ?? 0;



            ViewBag.ProduccionSemana =
                await producciones
                .Where(p =>
                p.fechaProduccion >= hoy.AddDays(-7))
                .SumAsync(p =>
                (decimal?)p.cantidadLeche) ?? 0;



            ViewBag.ProduccionMes =
                await producciones
                .Where(p =>
                p.fechaProduccion >= inicioMes)
                .SumAsync(p =>
                (decimal?)p.cantidadLeche) ?? 0;

            // ==============================
            // DATOS PARA GRÁFICO PRODUCCIÓN
            // ==============================

            var produccionGrafico = await producciones
                .Where(p => p.fechaProduccion >= hoy.AddDays(-7))
                .OrderBy(p => p.fechaProduccion)
                .ToListAsync();


            ViewBag.Labels = produccionGrafico
                .Select(p => p.fechaProduccion.ToString("dd/MM"))
                .ToList();


            ViewBag.Litros = produccionGrafico
                .Select(p => p.cantidadLeche)
                .ToList();



            // ==============================
            // ESTADO ANIMALES
            // ==============================


            ViewBag.AnimalesActivos =
                await animales.CountAsync(a =>
                a.Estado == EstadoAnimal.ACTIVO);



            ViewBag.AnimalesEnfermos =
                await animales.CountAsync(a =>
                a.Estado == EstadoAnimal.ENFERMO);



            ViewBag.AnimalesObservacion =
                await animales.CountAsync(a =>
                a.Estado == EstadoAnimal.OBSERVACION);





            // ==============================
            // ALIMENTOS
            // ==============================


            ViewBag.StockBajo =
                await _context.AlimentosBovinos
                .CountAsync(a =>
                a.CantidadDisponible <= a.NivelAlerta);





            // ==============================
            // PARTOS
            // ==============================


            ViewBag.ProximosPartos =
                await partos
                .CountAsync(p =>
                p.FechaParto >= hoy);



            ViewBag.ListaPartos =
                await partos
                .OrderBy(p =>
                p.FechaParto)
                .Take(5)
                .ToListAsync();





            // ==============================
            // ULTIMOS ANIMALES
            // ==============================


            ViewBag.UltimosAnimales =
                await animales
                .OrderByDescending(a =>
                a.IdAnimal)
                .Take(5)
                .ToListAsync();




            // ==============================
            // PERMISOS PARA DASHBOARD
            // ==============================


            ViewBag.Rol = rol;


            ViewBag.EsSuperAdmin =
                rol == "SUPER_ADMINISTRADOR";


            ViewBag.EsAdmin =
                rol == "ADMINISTRADOR";


            ViewBag.EsVeterinario =
                rol == "VETERINARIO";


            ViewBag.EsOperario =
                rol == "OPERARIO";

            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ??
                            HttpContext.TraceIdentifier
            });
        }

        [HttpGet]
        public IActionResult KeepAlive()
        {
            HttpContext.Session.SetString("KeepAlive", DateTime.Now.ToString());
            return Ok();
        }
    }
}