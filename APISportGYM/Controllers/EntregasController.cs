using ApiSportGYM.DTOs;
using APISportGYM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntregasController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public EntregasController(SportFitDbContext context)
        {
            _context = context;
        }


        // GET: api/entregas
        [HttpGet]
        public async Task<IActionResult> GetEntregas()
        {
            var entregas = await _context.Entregas
                .AsNoTracking()
                .OrderByDescending(e => e.FechaAsignacion)
                .Select(e => new
                {
                    e.IdEntrega,
                    e.FechaAsignacion,
                    e.FechaEntrega,
                    e.EstadoEntrega,
                    e.Observaciones,

                    Pedido = new
                    {
                        e.Pedido!.IdPedido,
                        e.Pedido.Estado,
                        e.Pedido.Total,

                        Cliente = new
                        {
                            e.Pedido.Cliente!.IdUsuario,
                            e.Pedido.Cliente.Nombre,
                            e.Pedido.Cliente.Apellido,
                            e.Pedido.Cliente.Telefono
                        }
                    },

                    Repartidor = new
                    {
                        e.Repartidor!.IdUsuario,
                        e.Repartidor.Nombre,
                        e.Repartidor.Apellido,
                        e.Repartidor.Telefono
                    }
                })
                .ToListAsync();

            return Ok(entregas);
        }


        // GET: api/entregas/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEntrega(int id)
        {
            var entrega = await _context.Entregas
                .AsNoTracking()
                .Where(e => e.IdEntrega == id)
                .Select(e => new
                {
                    e.IdEntrega,
                    e.FechaAsignacion,
                    e.FechaEntrega,
                    e.EstadoEntrega,
                    e.Observaciones,

                    Pedido = new
                    {
                        e.Pedido!.IdPedido,
                        e.Pedido.Estado,
                        e.Pedido.Subtotal,
                        e.Pedido.CostoEnvio,
                        e.Pedido.Total,

                        Cliente = new
                        {
                            e.Pedido.Cliente!.IdUsuario,
                            e.Pedido.Cliente.Nombre,
                            e.Pedido.Cliente.Apellido,
                            e.Pedido.Cliente.Correo,
                            e.Pedido.Cliente.Telefono
                        }
                    },

                    Repartidor = new
                    {
                        e.Repartidor!.IdUsuario,
                        e.Repartidor.Nombre,
                        e.Repartidor.Apellido,
                        e.Repartidor.Correo,
                        e.Repartidor.Telefono
                    }
                })
                .FirstOrDefaultAsync();

            if (entrega == null)
            {
                return NotFound(new
                {
                    mensaje = "La entrega no existe."
                });
            }

            return Ok(entrega);
        }


        // GET: api/entregas/pedido/id
        [HttpGet("pedido/{idPedido:int}")]
        public async Task<IActionResult> GetEntregaPorPedido(int idPedido)
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

            var entrega = await _context.Entregas
                .AsNoTracking()
                .Where(e => e.IdPedido == idPedido)
                .Select(e => new
                {
                    e.IdEntrega,
                    e.FechaAsignacion,
                    e.FechaEntrega,
                    e.EstadoEntrega,
                    e.Observaciones,

                    Repartidor = new
                    {
                        e.Repartidor!.IdUsuario,
                        e.Repartidor.Nombre,
                        e.Repartidor.Apellido,
                        e.Repartidor.Telefono
                    }
                })
                .FirstOrDefaultAsync();

            if (entrega == null)
            {
                return NotFound(new
                {
                    mensaje = "El pedido todavía no tiene una entrega asignada."
                });
            }

            return Ok(entrega);
        }


        // GET: api/entregas/repartidor/id(repartidor)
        [HttpGet("repartidor/{idRepartidor:int}")]
        public async Task<IActionResult> GetEntregasRepartidor(
            int idRepartidor)
        {
            var repartidorExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.IdUsuario == idRepartidor &&
                    u.Estado &&
                    u.Rol != null &&
                    u.Rol.Nombre == "Repartidor");

            if (!repartidorExiste)
            {
                return NotFound(new
                {
                    mensaje = "El repartidor no existe o está inactivo."
                });
            }

            var entregas = await _context.Entregas
                .AsNoTracking()
                .Where(e => e.IdRepartidor == idRepartidor)
                .OrderByDescending(e => e.FechaAsignacion)
                .Select(e => new
                {
                    e.IdEntrega,
                    e.FechaAsignacion,
                    e.FechaEntrega,
                    e.EstadoEntrega,
                    e.Observaciones,

                    Pedido = new
                    {
                        e.Pedido!.IdPedido,
                        e.Pedido.Total,

                        Cliente = new
                        {
                            e.Pedido.Cliente!.Nombre,
                            e.Pedido.Cliente.Apellido,
                            e.Pedido.Cliente.Telefono
                        }
                    }
                })
                .ToListAsync();

            return Ok(entregas);
        }


        // POST: api/entregas
        [HttpPost]
        public async Task<IActionResult> CrearEntrega(
            EntregaCrearDto dto)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_AsignarEntrega
                        @IdPedido,
                        @IdRepartidor,
                        @Observaciones",

                    new SqlParameter(
                        "@IdPedido",
                        dto.IdPedido),

                    new SqlParameter(
                        "@IdRepartidor",
                        dto.IdRepartidor),

                    new SqlParameter(
                        "@Observaciones",
                        (object?)dto.Observaciones?.Trim()
                        ?? DBNull.Value)
                );

                var entrega = await _context.Entregas
                    .AsNoTracking()
                    .Where(e => e.IdPedido == dto.IdPedido)
                    .Select(e => new
                    {
                        e.IdEntrega,
                        e.IdPedido,
                        e.IdRepartidor,
                        e.DireccionEntrega,
                        e.FechaAsignacion,
                        e.EstadoEntrega,
                        e.Observaciones
                    })
                    .FirstAsync();

                return CreatedAtAction(
                    nameof(GetEntrega),
                    new { id = entrega.IdEntrega },
                    entrega
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


        // PUT: api/entregas/id/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> ActualizarEstadoEntrega(
            int id,
            EntregaEstadoDto dto)
        {
            var entrega = await _context.Entregas
                .Include(e => e.Pedido)
                .FirstOrDefaultAsync(e => e.IdEntrega == id);

            if (entrega == null)
            {
                return NotFound(new
                {
                    mensaje = "La entrega no existe."
                });
            }

            entrega.EstadoEntrega = dto.EstadoEntrega;

            if (dto.EstadoEntrega == "En camino")
            {
                if (entrega.Pedido != null)
                {
                    entrega.Pedido.Estado = "Enviado";
                }
            }

            if (dto.EstadoEntrega == "Entregada")
            {
                entrega.FechaEntrega = DateTime.Now;

                if (entrega.Pedido != null)
                {
                    entrega.Pedido.Estado = "Entregado";
                }
            }
            else if (dto.EstadoEntrega != "Entregada")
            {
                entrega.FechaEntrega = null;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado de entrega actualizado correctamente.",
                entrega.IdEntrega,
                entrega.EstadoEntrega,
                entrega.FechaEntrega
            });
        }


        // PUT: api/entregas/id/repartidor
        [HttpPut("{id:int}/repartidor")]
        public async Task<IActionResult> ReasignarRepartidor(
            int id,
            EntregaRepartidorDto dto)
        {
            var entrega = await _context.Entregas
                .FindAsync(id);

            if (entrega == null)
            {
                return NotFound(new
                {
                    mensaje = "La entrega no existe."
                });
            }

            if (entrega.EstadoEntrega == "Entregada")
            {
                return BadRequest(new
                {
                    mensaje =
                        "No se puede reasignar una entrega que ya fue entregada."
                });
            }

            var repartidorExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.IdUsuario == dto.IdRepartidor &&
                    u.Estado &&
                    u.Rol != null &&
                    u.Rol.Nombre == "Repartidor");

            if (!repartidorExiste)
            {
                return BadRequest(new
                {
                    mensaje =
                        "El usuario indicado no es un repartidor activo."
                });
            }

            entrega.IdRepartidor = dto.IdRepartidor;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Repartidor reasignado correctamente.",
                entrega.IdEntrega,
                entrega.IdRepartidor
            });
        }
    }
}