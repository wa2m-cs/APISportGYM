namespace APISportGYM.Models
{
    public class Auditoria
    {
        public long IdAuditoria { get; set; }

        public string TablaAfectada { get; set; } = string.Empty;

        public int? IdRegistro { get; set; }

        public string Accion { get; set; } = string.Empty;

        public string? ValorAnterior { get; set; }

        public string? ValorNuevo { get; set; }

        public DateTime FechaCambio { get; set; }

        public int? IdUsuarioAccion { get; set; }

        public string UsuarioBD { get; set; } = string.Empty;
    }
}