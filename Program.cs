using Microsoft.EntityFrameworkCore;
using pc2.Data;

var builder = WebApplication.CreateBuilder(args);

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
