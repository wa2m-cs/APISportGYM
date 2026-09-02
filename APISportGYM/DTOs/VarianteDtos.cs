using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class VarianteCrearDto
    {
        [Required]
        [MaxLength(20)]
        public string Talla { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [MaxLength(60)]
        public string SKU { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, int.MaxValue)]
        public int StockMinimo { get; set; } = 3;
    }

    public class VarianteActualizarDto
    {
        [Required]
        [MaxLength(20)]
        public string Talla { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        [Required]
        [MaxLength(60)]
        public string SKU { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int StockMinimo { get; set; } = 3;
    }

    public class StockActualizarDto
    {
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}