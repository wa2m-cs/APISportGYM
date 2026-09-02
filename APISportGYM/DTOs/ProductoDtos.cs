using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class ProductoCrearDto
    {
        [Required]
        [MaxLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, 99999999)]
        public decimal Precio { get; set; }

        [MaxLength(100)]
        public string? Marca { get; set; }

        [RegularExpression("Hombre|Mujer|Unisex",
            ErrorMessage = "El género debe ser Hombre, Mujer o Unisex.")]
        public string? Genero { get; set; }

        [MaxLength(500)]
        public string? Imagen { get; set; }

        [Range(1, int.MaxValue)]
        public int IdCategoria { get; set; }
    }

    public class ProductoActualizarDto
    {
        [Required]
        [MaxLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Range(0, 99999999)]
        public decimal Precio { get; set; }

        [MaxLength(100)]
        public string? Marca { get; set; }

        [RegularExpression("Hombre|Mujer|Unisex",
            ErrorMessage = "El género debe ser Hombre, Mujer o Unisex.")]
        public string? Genero { get; set; }

        [MaxLength(500)]
        public string? Imagen { get; set; }

        public bool Estado { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int IdCategoria { get; set; }
    }
}