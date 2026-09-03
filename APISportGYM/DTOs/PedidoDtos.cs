using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class PedidoCrearDto
    {
        [Range(1, int.MaxValue)]
        public int IdCliente { get; set; }

        [Range(0, 99999999)]
        public decimal CostoEnvio { get; set; }

        public string DireccionEntrega { get; set; } = string.Empty;
    }


    public class PedidoEstadoDto
    {
        [Required]
        [RegularExpression(
            "Pendiente|Confirmado|Preparando|Listo|Enviado|Entregado|Cancelado",
            ErrorMessage = "Estado de pedido no válido.")]
        public string Estado { get; set; } = string.Empty;
    }

    public class DetallePedidoCrearDto
    {
        [Range(1, int.MaxValue)]
        public int IdVariante { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }

    public class DetallePedidoActualizarDto
    {
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }
    }
}