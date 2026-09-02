namespace APISportGYM.Models
{
    public class DetallePedido
    {
        public int IdDetalle { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }

        public int IdPedido { get; set; }

        public int IdVariante { get; set; }

        public Pedido? Pedido { get; set; }

        public VarianteProducto? Variante { get; set; }
    }
}