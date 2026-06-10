using Microsoft.AspNetCore.Mvc;
using CampoVerde.Data;
using Microsoft.EntityFrameworkCore;
using CampoVerde.Models;

namespace CampoVerde.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AppDbContext _context;

        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var animals = await _context.Animales.ToListAsync();
            return View(animals);
        }

        public async Task<IActionResult> Details(int id)
        {
            var animal = await _context.Animales.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(Animal animal)
        {
            if (ModelState.IsValid)
            {
                // Npgsql requires DateTime.Kind == Utc when writing to timestamptz
                if (animal.fechaNacimiento.HasValue)
                {
                    animal.fechaNacimiento = DateTime.SpecifyKind(animal.fechaNacimiento.Value, DateTimeKind.Utc);
                }

                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

    }
}
