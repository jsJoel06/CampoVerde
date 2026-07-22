using Microsoft.EntityFrameworkCore;
using CampoVerde.Data;

var builder = WebApplication.CreateBuilder(args);

// --- ESTA ES LA LÍNEA QUE TE FALTA ---
builder.Services.AddHttpContextAccessor();
// -------------------------------------

// Configuración de la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Servicios MVC
builder.Services.AddControllersWithViews();


// Agregar soporte para sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar sesiones (Esto es correcto, debe ir antes de app.MapControllerRoute)
app.UseSession();

app.UseAuthorization();

// Mapa de rutas estándar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();