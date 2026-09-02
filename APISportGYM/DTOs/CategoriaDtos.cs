using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class CategoriaCrearDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }
    }

    public class CategoriaActualizarDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true;
    }
}