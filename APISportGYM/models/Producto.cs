namespace APISportGYM.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public string? Marca { get; set; }

        public string? Genero { get; set; }

        public string? Imagen { get; set; }

        public bool Estado { get; set; }

        public int IdCategoria { get; set; }

        public Categoria? Categoria { get; set; }

        public ICollection<VarianteProducto> Variantes { get; set; }
            = new List<VarianteProducto>();
    }
}