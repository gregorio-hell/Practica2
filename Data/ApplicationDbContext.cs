using Microsoft.EntityFrameworkCore;
using pc2.Models;

namespace pc2.Data
{
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
            public ApplicationDbContext() { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder.UseSqlite("Data Source=app.db");
                }
            }

            public DbSet<Inmueble> Inmuebles { get; set; }
            public DbSet<Visita> Visitas { get; set; }
            public DbSet<Reserva> Reservas { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Codigo único
                modelBuilder.Entity<Inmueble>().HasIndex(i => i.Codigo).IsUnique();
                // Restricción: Precio > 0, MetrosCuadrados > 0
                modelBuilder.Entity<Inmueble>().HasCheckConstraint("CK_Inmueble_Precio", "Precio > 0");
                modelBuilder.Entity<Inmueble>().HasCheckConstraint("CK_Inmueble_MetrosCuadrados", "MetrosCuadrados > 0");
                // Restricción: FechaInicio < FechaFin
                modelBuilder.Entity<Visita>().HasCheckConstraint("CK_Visita_Fechas", "FechaInicio < FechaFin");
                // No permitir dos visitas solapadas para el mismo inmueble (validación en lógica de negocio)
                // Un inmueble no puede tener más de una reserva activa (validación en lógica de negocio)
                // Semilla mínima
                modelBuilder.Entity<Inmueble>().HasData(
                    new Inmueble { Id = 1, Codigo = "DEP001", Titulo = "Departamento céntrico", Imagen = "dep1.jpg", Tipo = TipoInmueble.Departamento, Ciudad = "Lima", Direccion = "Av. Central 123", Dormitorios = 2, Banos = 2, MetrosCuadrados = 80, Precio = 120000, Activo = true },
                    new Inmueble { Id = 2, Codigo = "CASA002", Titulo = "Casa familiar", Imagen = "casa2.jpg", Tipo = TipoInmueble.Casa, Ciudad = "Arequipa", Direccion = "Calle Sur 45", Dormitorios = 3, Banos = 3, MetrosCuadrados = 150, Precio = 250000, Activo = true },
                    new Inmueble { Id = 3, Codigo = "OFI003", Titulo = "Oficina moderna", Imagen = "ofi3.jpg", Tipo = TipoInmueble.Oficina, Ciudad = "Cusco", Direccion = "Jr. Comercio 10", Dormitorios = 0, Banos = 1, MetrosCuadrados = 60, Precio = 90000, Activo = true },
                    new Inmueble { Id = 4, Codigo = "LOC004", Titulo = "Local comercial", Imagen = "loc4.jpg", Tipo = TipoInmueble.Local, Ciudad = "Trujillo", Direccion = "Av. Norte 77", Dormitorios = 0, Banos = 2, MetrosCuadrados = 100, Precio = 180000, Activo = true }
                );
            }
        }
}
