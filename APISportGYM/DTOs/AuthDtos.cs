using System.ComponentModel.DataAnnotations;

namespace ApiSportGYM.DTOs
{
    public class RegistroDto
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

        public DateTime? FechaNacimiento { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;
    }
}