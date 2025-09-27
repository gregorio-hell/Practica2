using Microsoft.EntityFrameworkCore;
using pc2.Data;

var builder = WebApplication.CreateBuilder(args);
// Configuración de Redis y sesión
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";
// Para configurar la cadena de conexión de Redis, usa la variable de entorno REDIS_CONNECTION o el valor en appsettings.json:
// Ejemplo en Windows PowerShell:
// $env:REDIS_CONNECTION = "localhost:6379"
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.UseSession();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalogo}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapControllerRoute(
    name: "catalogo",
    pattern: "Catalogo/{action=Index}/{id?}",
    defaults: new { controller = "Catalogo" });
app.MapControllerRoute(
    name: "visitasreservas",
    pattern: "VisitasReservas/{action=Index}/{id?}",
    defaults: new { controller = "VisitasReservas" });

app.Run();
