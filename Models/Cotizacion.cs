using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalculadoraImportacionesWeb.Models
{
    public class Cotizacion
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "CNY → USD")]
        public decimal CnyUsd { get; set; }

        [Display(Name = "USD → ARS")]
        public decimal UsdArs { get; set; }

        [Display(Name = "Es Fallback")]
        public bool EsFallback { get; set; } = false;

        [Display(Name = "CNY → ARS (Indirecto)")]
        public decimal CnyArsIndirecto => CnyUsd * UsdArs;

        [NotMapped]
        [Display(Name = "CNY/USD")]
        public string CnyUsdDisplay => $"{CnyUsd:N6}";

        [NotMapped]
        [Display(Name = "USD/ARS")]
        public string UsdArsDisplay => $"{UsdArs:N2}";
    }
}
