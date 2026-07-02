using Microsoft.EntityFrameworkCore;
using CampoVerde.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Servicios MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Importante para tus imágenes y CSS
app.UseRouting();
app.UseAuthorization();

// Mapa de rutas estándar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); 