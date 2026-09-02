using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class PagoCrearDto
    {
        [Range(1, int.MaxValue)]
        public int IdPedido { get; set; }

        [Required]
        [MaxLength(40)]
        public string MetodoPago { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Referencia { get; set; }
    }

    public class PagoEstadoDto
    {
        [Required]
        [RegularExpression(
            "Pendiente|Aprobado|Rechazado|Reembolsado",
            ErrorMessage = "Estado de pago no válido.")]
        public string EstadoPago { get; set; } = string.Empty;
    }
}