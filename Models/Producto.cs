using System;
using System.ComponentModel.DataAnnotations;

namespace CalculadoraImportacionesWeb.Models
{
    public class Producto
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MaxLength(255, ErrorMessage = "Máximo 255 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El valor FOB es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
        [Display(Name = "Valor FOB (¥)")]
        public decimal ValorFOBYuan { get; set; }

        [Required(ErrorMessage = "El peso unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor a 0")]
        [Display(Name = "Peso Unitario (kg)")]
        public decimal PesoUnitario { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe ser al menos 1")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "FOB Unit. (USD)")]
        public decimal ValorFOBUnitarioUSD { get; set; } = 0;

        [Display(Name = "FOB Total (USD)")]
        public decimal ValorTotalProductosUSD { get; set; } = 0;

        [Range(0, 1, ErrorMessage = "Debe estar entre 0% y 100%")]
        [Display(Name = "Arancel Aduana %")]
        public decimal PorcentajeArancelAduana { get; set; } = 0.30m;

        [Range(0, 1, ErrorMessage = "Debe estar entre 0% y 100%")]
        [Display(Name = "Arancel Estadísticas %")]
        public decimal PorcentajeArancelEstadisticas { get; set; } = 0.025m;

        [Range(0, 1, ErrorMessage = "Debe estar entre 0% y 100%")]
        [Display(Name = "IVA %")]
        public decimal IVA { get; set; } = 0.21m;

        public decimal ValorPA { get; set; } = 0;
        public decimal BaseImponible { get; set; } = 0;
        public decimal Derechos { get; set; } = 0;
        public decimal Estadisticas { get; set; } = 0;
        public decimal IVAImportacion { get; set; } = 0;

        public decimal Almacenaje { get; set; } = 0;
        public decimal Flete { get; set; } = 0;
        public decimal FSCSEC { get; set; } = 0;
        public decimal Seguro { get; set; } = 0;
        public decimal CargoTerminal { get; set; } = 0;

        public decimal CostoTotal { get; set; } = 0;
        public decimal CostoUnitarioUSD { get; set; } = 0;
        public decimal CostoARS { get; set; } = 0;

        [Display(Name = "Precio Venta (ARS)")]
        public decimal PrecioVentaARS { get; set; } = 0;

        public decimal PrecioSugeridoConCargosYGananciaARS { get; set; } = 0;
        public decimal CargosPorVentaARS { get; set; } = 0;

        public decimal Neto { get; set; } = 0;
        
        [Display(Name = "Ganancia %")]
        public decimal PorcentajeGanancia { get; set; } = 0;
        
        [Display(Name = "Fecha Ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        [MaxLength(50)]
        [Display(Name = "Nomenclatura")]
        public string NomenclaturaComun { get; set; } = string.Empty;
    }
}
