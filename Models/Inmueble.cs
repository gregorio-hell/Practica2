using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace pc2.Models
{
    public enum TipoInmueble
    {
        Departamento,
        Casa,
        Oficina,
        Local
    }

    public class Inmueble
    {
        public int Id { get; set; }
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Titulo { get; set; }
        public string Imagen { get; set; }
        [Required]
        public TipoInmueble Tipo { get; set; }
        [Required]
        public string Ciudad { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Range(0, int.MaxValue)]
        public int Dormitorios { get; set; }
        [Range(0, int.MaxValue)]
        public int Banos { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "MetrosCuadrados debe ser mayor a 0")]
        public double MetrosCuadrados { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Precio debe ser mayor a 0")]
        public double Precio { get; set; }
        public bool Activo { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
        public ICollection<Visita> Visitas { get; set; }
    }
}
