using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pc2.Data;
using pc2.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace pc2.Controllers
{
    [Authorize]
    public class VisitasReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VisitasReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> AgendarVisita(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null || !inmueble.Activo)
                return NotFound();
            ViewBag.Inmueble = inmueble;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgendarVisita(int id, DateTime FechaInicio, DateTime FechaFin, string Notas)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null || !inmueble.Activo)
                return NotFound();
            if (FechaInicio >= FechaFin)
                ModelState.AddModelError("", "La fecha de inicio debe ser menor que la fecha de fin.");
            if (FechaInicio.Hour < 8 || FechaFin.Hour > 19)
                ModelState.AddModelError("", "Las visitas solo pueden agendarse en horario laboral (08:00–19:00).");
            // Validar solapamiento
            var solapada = await _context.Visitas.AnyAsync(v => v.InmuebleId == id &&
                ((FechaInicio < v.FechaFin && FechaFin > v.FechaInicio)));
            if (solapada)
                ModelState.AddModelError("", "Ya existe una visita solapada en ese intervalo para este inmueble.");
            if (!ModelState.IsValid)
            {
                ViewBag.Inmueble = inmueble;
                return View();
            }
            var visita = new Visita
            {
                InmuebleId = id,
                UsuarioId = User.Identity.Name,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin,
                Estado = EstadoVisita.Solicitada,
                Notas = Notas
            };
            _context.Visitas.Add(visita);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Visita agendada correctamente.";
            return RedirectToAction("Detalle", "Catalogo", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Reservar(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null || !inmueble.Activo)
                return NotFound();
            var reservaActiva = await _context.Reservas.AnyAsync(r => r.InmuebleId == id && r.FechaExpiracion > DateTime.Now);
            if (reservaActiva)
            {
                TempData["Error"] = "Ya existe una reserva activa para este inmueble.";
                return RedirectToAction("Detalle", "Catalogo", new { id });
            }
            var reserva = new Reserva
            {
                InmuebleId = id,
                UsuarioId = User.Identity.Name,
                FechaCreacion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddHours(48)
            };
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Reserva realizada correctamente por 48 horas.";
            return RedirectToAction("Detalle", "Catalogo", new { id });
        }
    }
}
