using Microsoft.EntityFrameworkCore;
using CampoVerde.Models;

namespace CampoVerde.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Animal> Animales { get; set; }
        public DbSet<Vacuna> Vacunas { get; set; }
        public DbSet<Produccion> Producciones { get; set; }
        public DbSet<Parto> Partos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
    }
}