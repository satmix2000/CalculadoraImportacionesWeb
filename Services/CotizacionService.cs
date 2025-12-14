using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CalculadoraImportacionesWeb.Models;

namespace CalculadoraImportacionesWeb.Services
{
    public interface ICotizacionService
    {
        Task<Cotizacion> ObtenerAsync(bool forzarActualizacion = false);
    }

    public class CotizacionService : ICotizacionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _filePath;

        public CotizacionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            
            // Ruta para el archivo de respaldo en wwwroot
            _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cotizaciones.json");
            
            // Asegurar que el directorio existe
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
        }

        public async Task<Cotizacion> ObtenerAsync(bool forzarActualizacion = false)
        {
            try
            {
                Console.WriteLine("🔄 Intentando obtener cotización desde API...");
                
                // 1. Obtener CNY -> USD desde open.er-api.com
                string urlCny = "https://open.er-api.com/v6/latest/CNY";
                using var responseCny = await _httpClient.GetAsync(urlCny);
                responseCny.EnsureSuccessStatusCode();
                
                var jsonContentCny = await responseCny.Content.ReadAsStringAsync();
                using var docCny = JsonDocument.Parse(jsonContentCny);
                var rootCny = docCny.RootElement;
                double tasaCnyUsd = rootCny.GetProperty("rates").GetProperty("USD").GetDouble();

                // 2. Obtener USD -> ARS desde dolarapi.com
                string urlUsdArs = "https://dolarapi.com/v1/dolares/oficial";
                using var responseUsdArs = await _httpClient.GetAsync(urlUsdArs);
                responseUsdArs.EnsureSuccessStatusCode();
                
                var jsonContentUsdArs = await responseUsdArs.Content.ReadAsStringAsync();
                using var docUsdArs = JsonDocument.Parse(jsonContentUsdArs);
                var rootUsdArs = docUsdArs.RootElement;
                double tasaUsdArs = rootUsdArs.GetProperty("venta").GetDouble();

                var cotizacion = new Cotizacion
                {
                    Fecha = DateTime.Now,
                    CnyUsd = (decimal)tasaCnyUsd,
                    UsdArs = (decimal)tasaUsdArs,
                    EsFallback = false
                };

                // Guardar en archivo para futuros fallbacks
                await GuardarCotizacionAsync(cotizacion);
                Console.WriteLine("✅ Cotización actualizada desde API.");
                
                return cotizacion;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al consultar API: {ex.Message}");
                
                // Intentar cargar desde archivo local
                var cotizacionDesdeArchivo = await CargarCotizacionGuardadaAsync();
                if (cotizacionDesdeArchivo != null)
                {
                    cotizacionDesdeArchivo.EsFallback = true;
                    Console.WriteLine("✅ Usando cotización guardada como fallback.");
                    return cotizacionDesdeArchivo;
                }

                // Si no hay archivo, usar valores por defecto
                Console.WriteLine("⚠️ Usando valores por defecto.");
                return new Cotizacion
                {
                    Fecha = DateTime.Now,
                    CnyUsd = 0.1388m,
                    UsdArs = 1025.50m,
                    EsFallback = true
                };
            }
        }

        private async Task GuardarCotizacionAsync(Cotizacion cotizacion)
        {
            try
            {
                string json = JsonSerializer.Serialize(cotizacion, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al guardar cotización: {ex.Message}");
            }
        }

        private async Task<Cotizacion?> CargarCotizacionGuardadaAsync()
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }

            try
            {
                string json = await File.ReadAllTextAsync(_filePath);
                return JsonSerializer.Deserialize<Cotizacion>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
