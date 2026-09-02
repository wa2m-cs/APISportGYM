using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace APISportGYM.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Contrasena { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public bool Estado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int IdRol { get; set; }

        public Rol? Rol { get; set; }

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
    }
}