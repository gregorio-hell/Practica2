using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pc2.Data;
using pc2.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;

namespace pc2.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para invalidar el caché de catálogo
        private async Task InvalidarCatalogoCacheAsync()
        {
            var cache = HttpContext?.RequestServices.GetService<IDistributedCache>();
            if (cache == null) return;
            // NOTA: IDistributedCache no soporta enumerar claves, así que solo se puede eliminar claves conocidas
            // Si tienes acceso a StackExchange.Redis, puedes eliminar por patrón. Aquí solo se muestra cómo eliminar una clave ejemplo.
            // Elimina la clave base, pero para eliminar todas las variantes deberías guardar las claves generadas o usar un script Redis.
            await cache.RemoveAsync("catalogo:");
        }

        public async Task<IActionResult> Index(string ciudad, TipoInmueble? tipo, double? precioMin, double? precioMax, int? dormitorios, int page = 1)
    {
        // Redis cache para listado de inmuebles
        var cacheKey = $"catalogo:{ciudad}:{tipo}:{precioMin}:{precioMax}:{dormitorios}:{page}";
        var cache = HttpContext.RequestServices.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
    List<Inmueble> inmuebles = new List<Inmueble>();
    int total = 0;
    int pageSize = 5;

        var cached = cache != null ? await cache.GetStringAsync(cacheKey) : null;
        if (cached != null)
        {
            inmuebles = System.Text.Json.JsonSerializer.Deserialize<List<Inmueble>>(cached) ?? new List<Inmueble>();
            // Para paginación correcta, se requiere contar todos los inmuebles activos con filtros
            var queryCount = _context.Inmuebles.Where(i => i.Activo);
            if (!string.IsNullOrEmpty(ciudad))
                queryCount = queryCount.Where(i => i.Ciudad == ciudad);
            if (tipo.HasValue)
                queryCount = queryCount.Where(i => i.Tipo == tipo);
            if (precioMin.HasValue && precioMin.Value >= 0)
                queryCount = queryCount.Where(i => i.Precio >= precioMin);
            if (precioMax.HasValue && precioMax.Value >= 0)
                queryCount = queryCount.Where(i => i.Precio <= precioMax);
            if (precioMin.HasValue && precioMax.HasValue && precioMin > precioMax)
                ModelState.AddModelError("", "El precio mínimo no puede ser mayor al máximo.");
            if (dormitorios.HasValue && dormitorios.Value >= 0)
                queryCount = queryCount.Where(i => i.Dormitorios >= dormitorios);
            total = await queryCount.CountAsync();
        }
        else
        {
            var query = _context.Inmuebles.Where(i => i.Activo);
            if (!string.IsNullOrEmpty(ciudad))
                query = query.Where(i => i.Ciudad == ciudad);
            if (tipo.HasValue)
                query = query.Where(i => i.Tipo == tipo);
            if (precioMin.HasValue && precioMin.Value >= 0)
                query = query.Where(i => i.Precio >= precioMin);
            if (precioMax.HasValue && precioMax.Value >= 0)
                query = query.Where(i => i.Precio <= precioMax);
            if (precioMin.HasValue && precioMax.HasValue && precioMin > precioMax)
                ModelState.AddModelError("", "El precio mínimo no puede ser mayor al máximo.");
            if (dormitorios.HasValue && dormitorios.Value >= 0)
                query = query.Where(i => i.Dormitorios >= dormitorios);

            total = await query.CountAsync();
            inmuebles = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            if (cache != null)
            {
                var cacheData = System.Text.Json.JsonSerializer.Serialize(inmuebles);
                await cache.SetStringAsync(cacheKey, cacheData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = System.TimeSpan.FromSeconds(60)
                });
            }
        }

        ViewBag.TotalPages = (int)System.Math.Ceiling(total / (double)pageSize);
        ViewBag.Page = page;
        ViewBag.Ciudad = ciudad;
        ViewBag.Tipo = tipo;
        ViewBag.PrecioMin = precioMin;
        ViewBag.PrecioMax = precioMax;
        ViewBag.Dormitorios = dormitorios;

        // Guardar filtros en sesión
        HttpContext.Session.SetString("FiltroCiudad", ciudad ?? "");
        HttpContext.Session.SetString("FiltroTipo", tipo?.ToString() ?? "");
        HttpContext.Session.SetString("FiltroPrecioMin", precioMin?.ToString() ?? "");
        HttpContext.Session.SetString("FiltroPrecioMax", precioMax?.ToString() ?? "");
        HttpContext.Session.SetString("FiltroDormitorios", dormitorios?.ToString() ?? "");
        // Obtener todas las ciudades únicas para el filtro
        var todasCiudades = await _context.Inmuebles.Where(i => i.Activo).Select(i => i.Ciudad).Distinct().OrderBy(c => c).ToListAsync();
        ViewBag.Ciudades = todasCiudades;

        return View(inmuebles);
    }

    [HttpPost]
    public async Task<IActionResult> Crear(Inmueble inmueble)
    {
        if (ModelState.IsValid)
        {
            _context.Inmuebles.Add(inmueble);
            await _context.SaveChangesAsync();
            await InvalidarCatalogoCacheAsync();
            return RedirectToAction("Index");
        }
        return View(inmueble);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(Inmueble inmueble)
    {
        if (ModelState.IsValid)
        {
            _context.Inmuebles.Update(inmueble);
            await _context.SaveChangesAsync();
            await InvalidarCatalogoCacheAsync();
            return RedirectToAction("Index");
        }
        return View(inmueble);
    }

    [HttpPost]
    public async Task<IActionResult> Activar(int id)
    {
        var inmueble = await _context.Inmuebles.FindAsync(id);
        if (inmueble != null)
        {
            inmueble.Activo = true;
            await _context.SaveChangesAsync();
            await InvalidarCatalogoCacheAsync();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Desactivar(int id)
    {
        var inmueble = await _context.Inmuebles.FindAsync(id);
        if (inmueble != null)
        {
            inmueble.Activo = false;
            await _context.SaveChangesAsync();
            await InvalidarCatalogoCacheAsync();
        }
        return RedirectToAction("Index");
    }


        
    }
}