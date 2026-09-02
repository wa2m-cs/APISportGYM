namespace APISportGYM.Models
{
    public class VarianteProducto
    {
        public int IdVariante { get; set; }

        public string Talla { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public int Stock { get; set; }

        public int StockMinimo { get; set; }

        public int IdProducto { get; set; }

        public Producto? Producto { get; set; }

        public ICollection<DetallePedido> DetallesPedido { get; set; }
            = new List<DetallePedido>();
    }
}