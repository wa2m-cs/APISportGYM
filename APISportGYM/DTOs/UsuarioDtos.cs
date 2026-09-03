using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class UsuarioCrearDto
    {
        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        [MaxLength(255)]
        public string Contrasena { get; set; } = string.Empty;

        [MaxLength(25)]
        public string? Telefono { get; set; }

        [Range(1, int.MaxValue)]
        public int IdRol { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }

    public class UsuarioActualizarDto
    {
        [Required]
        [MaxLength(80)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [MaxLength(25)]
        public string? Telefono { get; set; }

        public bool Estado { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int IdRol { get; set; }

        public DateTime? FechaNacimiento { get; set; }
    }
}