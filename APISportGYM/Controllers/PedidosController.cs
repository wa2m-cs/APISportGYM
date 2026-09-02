using ApiSportGYM.DTOs;
using APISportGYM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public PedidosController(SportFitDbContext context)
        {
            _context = context;
        }


        // GET: api/pedidos
        [HttpGet]
        public async Task<IActionResult> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .AsNoTracking()
                .OrderByDescending(p => p.FechaPedido)
                .Select(p => new
                {
                    p.IdPedido,
                    p.FechaPedido,
                    p.Estado,
                    p.Subtotal,
                    p.CostoEnvio,
                    p.Total,

                    Cliente = new
                    {
                        p.Cliente!.IdUsuario,
                        p.Cliente.Nombre,
                        p.Cliente.Apellido,
                        p.Cliente.Correo
                    },

                    Detalles = p.Detalles.Select(d => new
                    {
                        d.IdDetalle,
                        d.Cantidad,
                        d.PrecioUnitario,
                        d.Subtotal,

                        Variante = new
                        {
                            d.Variante!.IdVariante,
                            d.Variante.SKU,
                            d.Variante.Talla,
                            d.Variante.Color,

                            Producto = new
                            {
                                d.Variante.Producto!.IdProducto,
                                d.Variante.Producto.Nombre
                            }
                        }
                    })
                })
                .ToListAsync();

            return Ok(pedidos);
        }


        // GET: api/pedidos/id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.IdPedido == id)
                .Select(p => new
                {
                    p.IdPedido,
                    p.FechaPedido,
                    p.Estado,
                    p.Subtotal,
                    p.CostoEnvio,
                    p.Total,

                    Cliente = new
                    {
                        p.Cliente!.IdUsuario,
                        p.Cliente.Nombre,
                        p.Cliente.Apellido,
                        p.Cliente.Correo
                    },

                    Detalles = p.Detalles.Select(d => new
                    {
                        d.IdDetalle,
                        d.Cantidad,
                        d.PrecioUnitario,
                        d.Subtotal,

                        Variante = new
                        {
                            d.Variante!.IdVariante,
                            d.Variante.SKU,
                            d.Variante.Talla,
                            d.Variante.Color,

                            Producto = new
                            {
                                d.Variante.Producto!.IdProducto,
                                d.Variante.Producto.Nombre,
                                d.Variante.Producto.Imagen
                            }
                        }
                    }),

                    Pago = p.Pago == null
                        ? null
                        : new
                        {
                            p.Pago.IdPago,
                            p.Pago.Monto,
                            p.Pago.EstadoPago,
                            p.Pago.MetodoPago
                        },

                    Entrega = p.Entrega == null
                        ? null
                        : new
                        {
                            p.Entrega.IdEntrega,
                            p.Entrega.EstadoEntrega,
                            p.Entrega.DireccionEntrega
                        }
                })
                .FirstOrDefaultAsync();

            if (pedido == null)
            {
                return NotFound(new
                {
                    mensaje = "El pedido no existe."
                });
            }

            return Ok(pedido);
        }


        // GET: api/pedidos/cliente/id
        [HttpGet("cliente/{idCliente:int}")]
        public async Task<IActionResult> GetPedidosCliente(int idCliente)
        {
            var clienteExiste = await _context.Usuarios
                .AnyAsync(u => u.IdUsuario == idCliente);

            if (!clienteExiste)
            {
                return NotFound(new
                {
                    mensaje = "El cliente no existe."
                });
            }

            var pedidos = await _context.Pedidos
                .AsNoTracking()
                .Where(p => p.IdCliente == idCliente)
                .OrderByDescending(p => p.FechaPedido)
                .Select(p => new
                {
                    p.IdPedido,
                    p.FechaPedido,
                    p.Estado,
                    p.Subtotal,
                    p.CostoEnvio,
                    p.Total,

                    CantidadProductos = p.Detalles.Sum(d => d.Cantidad)
                })
                .ToListAsync();

            return Ok(pedidos);
        }


        // POST: api/pedidos
        [HttpPost]
        public async Task<IActionResult> CrearPedido(PedidoCrearDto dto)
        {
            try
            {
                var idPedido = new SqlParameter
                {
                    ParameterName = "@IdPedido",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_CrearPedido @IdCliente, @CostoEnvio, @IdPedido OUTPUT",
                    new SqlParameter("@IdCliente", dto.IdCliente),
                    new SqlParameter("@CostoEnvio", dto.CostoEnvio),
                    idPedido
                );

                var nuevoId = (int)idPedido.Value;

                return CreatedAtAction(
                    nameof(GetPedido),
                    new { id = nuevoId },
                    new
                    {
                        mensaje = "Pedido creado correctamente.",
                        idPedido = nuevoId
                    });
            }
            catch (SqlException ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }


        // PUT: api/pedidos/id/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> ActualizarEstado(
            int id,
            PedidoEstadoDto dto)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new
                {
                    mensaje = "El pedido no existe."
                });
            }

            pedido.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado del pedido actualizado.",
                estado = pedido.Estado
            });
        }


        // POST: api/pedidos/1/detalles
        [HttpPost("{id:int}/detalles")]
        public async Task<IActionResult> AgregarDetalle(
            int id,
            DetallePedidoCrearDto dto)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_AgregarDetallePedido @IdPedido, @IdVariante, @Cantidad",
                    new SqlParameter("@IdPedido", id),
                    new SqlParameter("@IdVariante", dto.IdVariante),
                    new SqlParameter("@Cantidad", dto.Cantidad)
                );

                var pedido = await _context.Pedidos
                    .AsNoTracking()
                    .Where(p => p.IdPedido == id)
                    .Select(p => new
                    {
                        p.IdPedido,
                        p.Subtotal,
                        p.CostoEnvio,
                        p.Total
                    })
                    .FirstAsync();

                return Ok(new
                {
                    mensaje = "Producto agregado al pedido.",
                    pedido
                });
            }
            catch (SqlException ex)
            {
                return Conflict(new
                {
                    mensaje = ex.Message
                });
            }
        }


        // PUT: api/pedidos/id/detalles/3 por ejemplo
        [HttpPut("{id:int}/detalles/{idDetalle:int}")]
        public async Task<IActionResult> ActualizarDetalle(
            int id,
            int idDetalle,
            DetallePedidoActualizarDto dto)
        {
            var pedido = await _context.Pedidos
                .FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new
                {
                    mensaje = "El pedido no existe."
                });
            }

            if (pedido.Estado != "Pendiente" &&
                pedido.Estado != "Confirmado")
            {
                return BadRequest(new
                {
                    mensaje = "El pedido ya no puede modificarse."
                });
            }

            var detalle = await _context.DetallesPedido
                .FirstOrDefaultAsync(d =>
                    d.IdDetalle == idDetalle &&
                    d.IdPedido == id);

            if (detalle == null)
            {
                return NotFound(new
                {
                    mensaje = "El detalle no existe en este pedido."
                });
            }

            detalle.Cantidad = dto.Cantidad;

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Cantidad actualizada correctamente."
                });
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException)
            {
                return Conflict(new
                {
                    mensaje = ex.InnerException.Message
                });
            }
        }


        // DELETE: api/pedidos/id/detalles/3 por ejemplo
        [HttpDelete("{id:int}/detalles/{idDetalle:int}")]
        public async Task<IActionResult> EliminarDetalle(
            int id,
            int idDetalle)
        {
            var pedido = await _context.Pedidos
                .FindAsync(id);

            if (pedido == null)
            {
                return NotFound(new
                {
                    mensaje = "El pedido no existe."
                });
            }

            if (pedido.Estado != "Pendiente" &&
                pedido.Estado != "Confirmado")
            {
                return BadRequest(new
                {
                    mensaje = "El pedido ya no puede modificarse."
                });
            }

            var detalle = await _context.DetallesPedido
                .FirstOrDefaultAsync(d =>
                    d.IdDetalle == idDetalle &&
                    d.IdPedido == id);

            if (detalle == null)
            {
                return NotFound(new
                {
                    mensaje = "El detalle no existe en este pedido."
                });
            }

            _context.DetallesPedido.Remove(detalle);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}