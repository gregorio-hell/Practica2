using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pc2.Data;
using pc2.Models;
using System.Linq;
using System.Threading.Tasks;

namespace pc2.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string ciudad, TipoInmueble? tipo, double? precioMin, double? precioMax, int? dormitorios, int page = 1)
        {
            // Guardar filtros en sesión
            HttpContext.Session.SetString("FiltroCiudad", ciudad ?? "");
            HttpContext.Session.SetString("FiltroTipo", tipo?.ToString() ?? "");
            HttpContext.Session.SetString("FiltroPrecioMin", precioMin?.ToString() ?? "");
            HttpContext.Session.SetString("FiltroPrecioMax", precioMax?.ToString() ?? "");
            HttpContext.Session.SetString("FiltroDormitorios", dormitorios?.ToString() ?? "");
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

            int pageSize = 5;
            var total = await query.CountAsync();
            var inmuebles = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.TotalPages = (int)System.Math.Ceiling(total / (double)pageSize);
            ViewBag.Page = page;
            ViewBag.Ciudad = ciudad;
            ViewBag.Tipo = tipo;
            ViewBag.PrecioMin = precioMin;
            ViewBag.PrecioMax = precioMax;
            ViewBag.Dormitorios = dormitorios;

            return View(inmuebles);
        }
        }

        public async Task<IActionResult> Detalle(int id)
        {
            HttpContext.Session.SetInt32("UltimoInmuebleId", id);
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null || !inmueble.Activo)
                return NotFound();
            HttpContext.Session.SetString("UltimoInmuebleTitulo", inmueble.Titulo);
            return View(inmueble);
        }

        [HttpPost]
        public IActionResult AgendarVisita(int id)
        {
            // Aquí iría la lógica para agendar visita
            TempData["Mensaje"] = "Visita agendada correctamente.";
            return RedirectToAction("Detalle", new { id });
        }
    }
}
