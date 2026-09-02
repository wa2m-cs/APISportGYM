using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class EntregaCrearDto
    {
        [Range(1, int.MaxValue)]
        public int IdPedido { get; set; }

        [Range(1, int.MaxValue)]
        public int IdRepartidor { get; set; }

        [Required]
        [MaxLength(300)]
        public string DireccionEntrega { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Observaciones { get; set; }
    }

    public class EntregaEstadoDto
    {
        [Required]
        [RegularExpression(
            "Asignada|En camino|Entregada|No entregada",
            ErrorMessage = "Estado de entrega no válido.")]
        public string EstadoEntrega { get; set; } = string.Empty;
    }

    public class EntregaRepartidorDto
    {
        [Range(1, int.MaxValue)]
        public int IdRepartidor { get; set; }
    }
}