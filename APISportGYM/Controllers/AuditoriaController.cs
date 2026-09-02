using APISportGYM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditoriaController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public AuditoriaController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/auditoria
        [HttpGet]
        public async Task<IActionResult> GetAuditoria()
        {
            var registros = await _context.Auditorias
                .AsNoTracking()
                .OrderByDescending(a => a.FechaCambio)
                .Select(a => new
                {
                    a.IdAuditoria,
                    a.TablaAfectada,
                    a.IdRegistro,
                    a.Accion,
                    a.ValorAnterior,
                    a.ValorNuevo,
                    a.FechaCambio,
                    a.IdUsuarioAccion,
                    a.UsuarioBD
                })
                .ToListAsync();

            return Ok(registros);
        }


        // GET: api/auditoria/id
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetAuditoriaPorId(long id)
        {
            var registro = await _context.Auditorias
                .AsNoTracking()
                .Where(a => a.IdAuditoria == id)
                .Select(a => new
                {
                    a.IdAuditoria,
                    a.TablaAfectada,
                    a.IdRegistro,
                    a.Accion,
                    a.ValorAnterior,
                    a.ValorNuevo,
                    a.FechaCambio,
                    a.IdUsuarioAccion,
                    a.UsuarioBD
                })
                .FirstOrDefaultAsync();

            if (registro == null)
            {
                return NotFound(new
                {
                    mensaje = "El registro de auditoría no existe."
                });
            }

            return Ok(registro);
        }


        // GET: api/auditoria/tabla/Producto
        [HttpGet("tabla/{tabla}")]
        public async Task<IActionResult> GetAuditoriaPorTabla(string tabla)
        {
            var registros = await _context.Auditorias
                .AsNoTracking()
                .Where(a => a.TablaAfectada == tabla)
                .OrderByDescending(a => a.FechaCambio)
                .Select(a => new
                {
                    a.IdAuditoria,
                    a.TablaAfectada,
                    a.IdRegistro,
                    a.Accion,
                    a.ValorAnterior,
                    a.ValorNuevo,
                    a.FechaCambio,
                    a.IdUsuarioAccion,
                    a.UsuarioBD
                })
                .ToListAsync();

            if (registros.Count == 0)
            {
                return NotFound(new
                {
                    mensaje = "No hay registros de auditoría para esa tabla."
                });
            }

            return Ok(registros);
        }


        // GET: api/auditoria/accion/UPDATE
        [HttpGet("accion/{accion}")]
        public async Task<IActionResult> GetAuditoriaPorAccion(string accion)
        {
            accion = accion.ToUpper();

            if (accion != "INSERT" &&
                accion != "UPDATE" &&
                accion != "DELETE")
            {
                return BadRequest(new
                {
                    mensaje = "La acción debe ser INSERT, UPDATE o DELETE."
                });
            }

            var registros = await _context.Auditorias
                .AsNoTracking()
                .Where(a => a.Accion == accion)
                .OrderByDescending(a => a.FechaCambio)
                .Select(a => new
                {
                    a.IdAuditoria,
                    a.TablaAfectada,
                    a.IdRegistro,
                    a.Accion,
                    a.ValorAnterior,
                    a.ValorNuevo,
                    a.FechaCambio,
                    a.IdUsuarioAccion,
                    a.UsuarioBD
                })
                .ToListAsync();

            return Ok(registros);
        }
    }
}