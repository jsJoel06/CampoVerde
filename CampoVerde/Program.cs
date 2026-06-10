using Microsoft.EntityFrameworkCore;
using CampoVerde.Data;
var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRA EL SERVICIO DEL CONTEXTO DE DATOS
// Esto le dice a tu app: "Cuando alguien necesite datos, usa esta cadena de conexión"
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Estos son los nuevos métodos de ASP.NET Core 9+ para assets
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
app.Run();
