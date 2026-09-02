using ApiSportGYM.DTOs;
using APISportGYM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public PagosController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/pagos
        [HttpGet]
        public async Task<IActionResult> GetPagos()
        {
            var pagos = await _context.Pagos
                .AsNoTracking()
                .OrderByDescending(p => p.FechaPago)
                .Select(p => new
                {
                    p.IdPago,
                    p.FechaPago,
                    p.Monto,
                    p.MetodoPago,
                    p.EstadoPago,
                    p.Referencia,

                    Pedido = new
                    {
                        p.Pedido!.IdPedido,
                        p.Pedido.Estado,
                        p.Pedido.Total,
                        p.Pedido.FechaPedido
                    }
                })
                .ToListAsync();

            return Ok(pagos);
        }


        // GET: api/pagos/id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPago(int id)
        {
            var pago = await _context.Pagos
                .AsNoTracking()
                .Where(p => p.IdPago == id)
                .Select(p => new
                {
                    p.IdPago,
                    p.FechaPago,
                    p.Monto,
                    p.MetodoPago,
                    p.EstadoPago,
                    p.Referencia,

                    Pedido = new
                    {
                        p.Pedido!.IdPedido,
                        p.Pedido.Estado,
                        p.Pedido.Subtotal,
                        p.Pedido.CostoEnvio,
                        p.Pedido.Total
                    }
                })
                .FirstOrDefaultAsync();

            if (pago == null)
            {
                return NotFound(new
                {
                    mensaje = "El pago no existe."
                });
            }

            return Ok(pago);
        }


        // GET: api/pagos/pedido/id
        [HttpGet("pedido/{idPedido:int}")]
        public async Task<IActionResult> GetPagoPorPedido(int idPedido)
        {
            var pedidoExiste = await _context.Pedidos
                .AnyAsync(p => p.IdPedido == idPedido);

            if (!pedidoExiste)
            {
                return NotFound(new
                {
                    mensaje = "El pedido no existe."
                });
            }

            var pago = await _context.Pagos
                .AsNoTracking()
                .Where(p => p.IdPedido == idPedido)
                .Select(p => new
                {
                    p.IdPago,
                    p.FechaPago,
                    p.Monto,
                    p.MetodoPago,
                    p.EstadoPago,
                    p.Referencia,
                    p.IdPedido
                })
                .FirstOrDefaultAsync();

            if (pago == null)
            {
                return NotFound(new
                {
                    mensaje = "El pedido todavía no tiene un pago registrado."
                });
            }

            return Ok(pago);
        }


        // POST: api/pagos
        [HttpPost]
        public async Task<IActionResult> RegistrarPago(PagoCrearDto dto)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_RegistrarPago @IdPedido, @MetodoPago, @Referencia",
                    new SqlParameter("@IdPedido", dto.IdPedido),
                    new SqlParameter("@MetodoPago", dto.MetodoPago.Trim()),
                    new SqlParameter(
                        "@Referencia",
                        (object?)dto.Referencia?.Trim() ?? DBNull.Value
                    )
                );

                var pago = await _context.Pagos
                    .AsNoTracking()
                    .Where(p => p.IdPedido == dto.IdPedido)
                    .Select(p => new
                    {
                        p.IdPago,
                        p.FechaPago,
                        p.Monto,
                        p.MetodoPago,
                        p.EstadoPago,
                        p.Referencia,
                        p.IdPedido
                    })
                    .FirstAsync();

                return CreatedAtAction(
                    nameof(GetPago),
                    new { id = pago.IdPago },
                    pago
                );
            }
            catch (SqlException ex)
            {
                return Conflict(new
                {
                    mensaje = ex.Message
                });
            }
        }


        // PUT: api/pagos/id/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> ActualizarEstadoPago(
            int id,
            PagoEstadoDto dto)
        {
            var pago = await _context.Pagos.FindAsync(id);

            if (pago == null)
            {
                return NotFound(new
                {
                    mensaje = "El pago no existe."
                });
            }

            pago.EstadoPago = dto.EstadoPago;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado del pago actualizado correctamente.",
                pago.IdPago,
                pago.EstadoPago
            });
        }
    }
}