using System;
using System.Linq;
using System.Threading.Tasks;
using CalculadoraImportacionesWeb.Data;
using CalculadoraImportacionesWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraImportacionesWeb.Services
{
    public interface ICalculadoraService
    {
        Task<Producto> CalcularProductoAsync(Producto producto, bool precioVentaModificadoManualmente = false);
        Task<Producto> CalcularProductoPorIdAsync(int id, bool precioVentaModificadoManualmente = false);
        Task<ResultadoCalculo> CalcularRapidoAsync(decimal valorFobYuan, decimal peso, int cantidad);
    }

    public class CalculadoraService : ICalculadoraService
    {
        private readonly AppDbContext _context;
        private readonly ICotizacionService _cotizacionService;

        public CalculadoraService(AppDbContext context, ICotizacionService cotizacionService)
        {
            _context = context;
            _cotizacionService = cotizacionService;
        }

        public async Task<Producto> CalcularProductoAsync(Producto producto, bool precioVentaModificadoManualmente = false)
        {
            // Cargar ajustes desde la base de datos
            var ajustes = await _context.Ajustes.FirstOrDefaultAsync();
            if (ajustes == null)
            {
                throw new InvalidOperationException("No se encontraron ajustes en la base de datos.");
            }

            // Obtener cotización actual
            var cotizacion = await _cotizacionService.ObtenerAsync();
            
            // Usar tasas de la cotización o de ajustes si la cotización es fallback
            decimal tasaUSDARS = cotizacion.EsFallback ? ajustes.TasaUSDARS : cotizacion.UsdArs;
            decimal tasaCNYUSD = cotizacion.EsFallback ? ajustes.TasaCNYUSD : cotizacion.CnyUsd;

            // 1. Calcular FOB en USD
            producto.ValorFOBUnitarioUSD = producto.ValorFOBYuan * tasaCNYUSD;
            producto.ValorTotalProductosUSD = producto.ValorFOBUnitarioUSD * producto.Cantidad;

            // 2. Calcular costos fijos
            producto.Almacenaje = producto.PesoUnitario * producto.Cantidad * ajustes.AlmacenajePorKg;
            producto.Flete = producto.PesoUnitario * producto.Cantidad * ajustes.FletePorKg;
            producto.FSCSEC = producto.Flete * ajustes.PorcentajeFscSec;
            producto.Seguro = producto.ValorTotalProductosUSD * ajustes.PorcentajeSeguro;
            producto.CargoTerminal = producto.ValorTotalProductosUSD * ajustes.PorcentajeCargoTerminal;

            // 3. Calcular valor P.A. y base imponible (simplificado)
            producto.ValorPA = producto.ValorTotalProductosUSD;
            
            // Calcular derechos y estadísticas
            producto.Derechos = producto.ValorPA * producto.PorcentajeArancelAduana;
            producto.Estadisticas = producto.ValorPA * producto.PorcentajeArancelEstadisticas;
            
            producto.BaseImponible = producto.ValorPA + producto.Derechos + producto.Estadisticas;
            producto.IVAImportacion = producto.BaseImponible * producto.IVA;

            // 4. Calcular costo total y unitario en USD
            producto.CostoTotal = producto.ValorPA + producto.Derechos + producto.Estadisticas +
                                  producto.IVAImportacion + producto.Almacenaje + producto.Flete +
                                  producto.FSCSEC + producto.Seguro + producto.CargoTerminal;

            producto.CostoUnitarioUSD = producto.Cantidad > 0 ? producto.CostoTotal / producto.Cantidad : 0;

            // 5. Convertir costo unitario a ARS
            producto.CostoARS = producto.CostoUnitarioUSD * tasaUSDARS;

            // 6. Calcular cargos por venta (MercadoLibre)
            CalcularCargosMercadoLibre(producto, ajustes);

            // 7. Calcular precio de venta
            if (!precioVentaModificadoManualmente)
            {
                producto.PrecioVentaARS = (producto.CostoARS + producto.CargosPorVentaARS) / 
                                          (1 - ajustes.MargenGananciaDeseado);
            }

            producto.PrecioSugeridoConCargosYGananciaARS = producto.PrecioVentaARS;

            // 8. Calcular ganancia
            producto.Neto = producto.PrecioVentaARS - producto.CostoARS - producto.CargosPorVentaARS;
            
            if (producto.PrecioVentaARS > 0)
            {
                producto.PorcentajeGanancia = (producto.Neto / producto.PrecioVentaARS) * 100;
            }

            producto.FechaIngreso = DateTime.Now;

            return producto;
        }

        private void CalcularCargosMercadoLibre(Producto producto, Ajustes ajustes)
        {
            // Comisión variable (porcentaje del precio de venta)
            decimal cargoPorVender = producto.PrecioSugeridoConCargosYGananciaARS * 
                                     ajustes.CargoMercadoLibrePorcentaje;

            // Costo fijo según rango de precio
            decimal costoFijoPorUnidad = 0m;
            decimal precio = producto.PrecioSugeridoConCargosYGananciaARS;

            if (precio < 15000m)
                costoFijoPorUnidad = ajustes.CargoMercadoLibreFijo1;
            else if (precio <= 25000m)
                costoFijoPorUnidad = ajustes.CargoMercadoLibreFijo2;
            else if (precio <= 33000m)
                costoFijoPorUnidad = ajustes.CargoMercadoLibreFijo3;

            producto.CargosPorVentaARS = cargoPorVender + costoFijoPorUnidad;
        }

        public async Task<Producto> CalcularProductoPorIdAsync(int id, bool precioVentaModificadoManualmente = false)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                throw new InvalidOperationException($"No se encontró el producto con ID {id}");
            }

            return await CalcularProductoAsync(producto, precioVentaModificadoManualmente);
        }

        public async Task<ResultadoCalculo> CalcularRapidoAsync(decimal valorFobYuan, decimal peso, int cantidad)
        {
            var producto = new Producto
            {
                ValorFOBYuan = valorFobYuan,
                PesoUnitario = peso,
                Cantidad = cantidad,
                Descripcion = "Cálculo rápido",
                PorcentajeArancelAduana = 0.30m,
                PorcentajeArancelEstadisticas = 0.025m,
                IVA = 0.21m
            };

            producto = await CalcularProductoAsync(producto);

            return new ResultadoCalculo
            {
                CostoTotalUSD = producto.CostoTotal,
                CostoUnitarioARS = producto.CostoARS,
                PrecioSugeridoARS = producto.PrecioSugeridoConCargosYGananciaARS,
                GananciaPorcentaje = producto.PorcentajeGanancia
            };
        }
    }

    // Clase para resultados rápidos
    public class ResultadoCalculo
    {
        public decimal CostoTotalUSD { get; set; }
        public decimal CostoUnitarioARS { get; set; }
        public decimal PrecioSugeridoARS { get; set; }
        public decimal GananciaPorcentaje { get; set; }
    }
}
