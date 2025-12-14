using Microsoft.EntityFrameworkCore;
using CalculadoraImportacionesWeb.Models;

namespace CalculadoraImportacionesWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Ajustes> Ajustes { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ajustes>().HasData(
                new Ajustes
                {
                    Id = 1,
                    TasaUSDARS = 1025.50m,
                    TasaCNYUSD = 0.1388m,
                    PorcentajeArancelAduana = 0.30m,
                    PorcentajeArancelEstadisticas = 0.025m,
                    IVA = 0.21m,
                    MargenGananciaDeseado = 0.35m,
                    PorcentajeSeguro = 0.0121m,
                    PorcentajeCargoTerminal = 0.10m,
                    AlmacenajePorKg = 1.21m,
                    FletePorKg = 17.00m,
                    PorcentajeFscSec = 0.25m,
                    CargoMercadoLibrePorcentaje = 0.16m,
                    CargoMercadoLibreFijo1 = 1115m,
                    CargoMercadoLibreFijo2 = 2300m,
                    CargoMercadoLibreFijo3 = 2810m,
                    UltimaActualizacion = DateTime.Now,
                    ActualizadoPor = "Sistema"
                }
            );
        }
    }
}
