namespace APISportGYM.Models
{
    public class Entrega
    {
        public int IdEntrega { get; set; }

        public string DireccionEntrega { get; set; } = string.Empty;

        public DateTime FechaAsignacion { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public string EstadoEntrega { get; set; } = string.Empty;

        public string? Observaciones { get; set; }

        public int IdPedido { get; set; }

        public int IdRepartidor { get; set; }

        public Pedido? Pedido { get; set; }

        public Usuario? Repartidor { get; set; }
    }
}