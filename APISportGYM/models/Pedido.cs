namespace APISportGYM.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }

        public DateTime FechaPedido { get; set; }

        public string Estado { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal CostoEnvio { get; set; }

        public decimal Total { get; set; }

        public int IdCliente { get; set; }

        public Usuario? Cliente { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; }
            = new List<DetallePedido>();

        public Pago? Pago { get; set; }
        public string? DireccionEntrega { get; set; }

        public Entrega? Entrega { get; set; }
    }
}