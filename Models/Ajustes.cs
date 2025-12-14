using System;
using System.ComponentModel.DataAnnotations;

namespace CalculadoraImportacionesWeb.Models
{
    public class Ajustes
    {
        [Key]
        public int Id { get; set; } = 1;

        [Display(Name = "USD → ARS")]
        [Range(0.01, 10000, ErrorMessage = "Tasa inválida")]
        public decimal TasaUSDARS { get; set; } = 1025.50m;

        [Display(Name = "CNY → USD")]
        [Range(0.0001, 10, ErrorMessage = "Tasa inválida")]
        public decimal TasaCNYUSD { get; set; } = 0.1388m;

        // Porcentajes (se guardan como decimales: 0.30 = 30%)
        [Display(Name = "Arancel Aduana")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal PorcentajeArancelAduana { get; set; } = 0.30m;

        [Display(Name = "Arancel Estadísticas")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal PorcentajeArancelEstadisticas { get; set; } = 0.025m;

        [Display(Name = "IVA")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal IVA { get; set; } = 0.21m;

        [Display(Name = "Margen Ganancia")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal MargenGananciaDeseado { get; set; } = 0.35m;

        // Costos fijos
        [Display(Name = "Seguro")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal PorcentajeSeguro { get; set; } = 0.0121m;

        [Display(Name = "Cargo Terminal")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal PorcentajeCargoTerminal { get; set; } = 0.10m;

        [Display(Name = "Almacenaje (USD/kg)")]
        [Range(0, 1000, ErrorMessage = "Valor inválido")]
        public decimal AlmacenajePorKg { get; set; } = 1.21m;

        [Display(Name = "Flete (USD/kg)")]
        [Range(0, 1000, ErrorMessage = "Valor inválido")]
        public decimal FletePorKg { get; set; } = 17.00m;

        [Display(Name = "FSC/SEC")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal PorcentajeFscSec { get; set; } = 0.25m;

        // MercadoLibre
        [Display(Name = "Comisión ML")]
        [Range(0, 1, ErrorMessage = "0% a 100%")]
        public decimal CargoMercadoLibrePorcentaje { get; set; } = 0.16m;

        [Display(Name = "Costo Fijo 1 (< 15k ARS)")]
        public decimal CargoMercadoLibreFijo1 { get; set; } = 1115m;

        [Display(Name = "Costo Fijo 2 (15-25k ARS)")]
        public decimal CargoMercadoLibreFijo2 { get; set; } = 2300m;

        [Display(Name = "Costo Fijo 3 (25-33k ARS)")]
        public decimal CargoMercadoLibreFijo3 { get; set; } = 2810m;

        // Auditoría
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;
        public string ActualizadoPor { get; set; } = "Sistema";

        // Propiedades para mostrar (PORCENTAJES AMIGABLES)
        [NotMapped]
        [Display(Name = "Arancel Aduana")]
        public string PorcentajeArancelAduanaDisplay => $"{PorcentajeArancelAduana * 100:N1}%";

        [NotMapped]
        [Display(Name = "IVA")]
        public string IVADisplay => $"{IVA * 100:N1}%";

        [NotMapped]
        [Display(Name = "Margen Objetivo")]
        public string MargenDisplay => $"{MargenGananciaDeseado * 100:N1}%";
    }
}
