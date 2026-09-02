namespace APISportGYM.Models
{
    public class Pago
    {
        public int IdPago { get; set; }

        public DateTime FechaPago { get; set; }

        public decimal Monto { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public string EstadoPago { get; set; } = string.Empty;

        public string? Referencia { get; set; }

        public int IdPedido { get; set; }

        public Pedido? Pedido { get; set; }
    }
}