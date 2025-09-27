using System.ComponentModel.DataAnnotations;

namespace pc2.Models
{
    public enum EstadoVisita
    {
        Solicitada,
        Confirmada,
        Cancelada
    }

    public class Visita
    {
        public int Id { get; set; }
        [Required]
        public int InmuebleId { get; set; }
        public Inmueble Inmueble { get; set; }
        [Required]
        public string UsuarioId { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public EstadoVisita Estado { get; set; }
        public string Notas { get; set; }
    }
}
