using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pc2.Data;
using pc2.Models;
using pc2.Models.Constants;

namespace pc2.Controllers
{
    [Authorize(Roles = Roles.Broker)]
    public class BrokerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrokerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Broker
        public async Task<IActionResult> Index()
        {
            var inmuebles = await _context.Inmuebles.ToListAsync();
            return View(inmuebles);
        }

        // GET: /Broker/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Broker/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Titulo,Imagen,Precio,Ciudad,Direccion,Tipo,Dormitorios,Activo")] Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inmueble);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Inmueble creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // GET: /Broker/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: /Broker/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Titulo,Imagen,Precio,Ciudad,Direccion,Tipo,Dormitorios,Activo")] Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inmueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InmuebleExists(inmueble.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // POST: /Broker/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            inmueble.Activo = !inmueble.Activo;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Broker/Agenda
        public async Task<IActionResult> Agenda()
        {
            var hoy = DateTime.Today;
            var visitas = await _context.Visitas
                .Include(v => v.Inmueble)
                .Where(v => v.FechaInicio.Date == hoy)
                .OrderBy(v => v.FechaInicio)
                .ToListAsync();
            return View(visitas);
        }

        // POST: /Broker/ConfirmarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarVisita(int id)
        {
            var visita = await _context.Visitas.FindAsync(id);
            if (visita == null)
            {
                return NotFound();
            }

            visita.Estado = EstadoVisita.Confirmada;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Agenda));
        }

        // POST: /Broker/CancelarVisita/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarVisita(int id)
        {
            var visita = await _context.Visitas.FindAsync(id);
            if (visita == null)
            {
                return NotFound();
            }

            visita.Estado = EstadoVisita.Cancelada;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Agenda));
        }

        // GET: /Broker/Reservas
        public async Task<IActionResult> Reservas()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Inmueble)
                .Where(r => r.FechaExpiracion > DateTime.Now)
                .ToListAsync();
            return View(reservas);
        }

        // POST: /Broker/LiberarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LiberarReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            reserva.FechaExpiracion = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Reservas));
        }

        // GET: /Broker/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        private bool InmuebleExists(int id)
        {
            return _context.Inmuebles.Any(e => e.Id == id);
        }
    }
}