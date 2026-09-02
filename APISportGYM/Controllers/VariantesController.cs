using ApiSportGYM.DTOs;
using APISportGYM.Data;
using APISportGYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantesController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public VariantesController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/variantes
        [HttpGet]
        public async Task<IActionResult> GetVariantes()
        {
            var variantes = await _context.VariantesProducto
                .AsNoTracking()
                .Select(v => new
                {
                    v.IdVariante,
                    v.Talla,
                    v.Color,
                    v.SKU,
                    v.Stock,
                    v.StockMinimo,

                    Producto = new
                    {
                        v.Producto!.IdProducto,
                        v.Producto.Nombre,
                        v.Producto.Precio
                    }
                })
                .ToListAsync();

            return Ok(variantes);
        }

        // GET: api/variantes/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVariante(int id)
        {
            var variante = await _context.VariantesProducto
                .AsNoTracking()
                .Where(v => v.IdVariante == id)
                .Select(v => new
                {
                    v.IdVariante,
                    v.Talla,
                    v.Color,
                    v.SKU,
                    v.Stock,
                    v.StockMinimo,

                    Producto = new
                    {
                        v.Producto!.IdProducto,
                        v.Producto.Nombre,
                        v.Producto.Precio
                    }
                })
                .FirstOrDefaultAsync();

            if (variante == null)
            {
                return NotFound(new
                {
                    mensaje = "La variante no existe."
                });
            }

            return Ok(variante);
        }

        // GET: api/productos/1/variantes
        [HttpGet("/api/productos/{idProducto}/variantes")]
        public async Task<IActionResult> GetVariantesProducto(int idProducto)
        {
            var productoExiste = await _context.Productos
                .AnyAsync(p => p.IdProducto == idProducto);

            if (!productoExiste)
            {
                return NotFound(new
                {
                    mensaje = "El producto no existe."
                });
            }

            var variantes = await _context.VariantesProducto
                .AsNoTracking()
                .Where(v => v.IdProducto == idProducto)
                .Select(v => new
                {
                    v.IdVariante,
                    v.Talla,
                    v.Color,
                    v.SKU,
                    v.Stock,
                    v.StockMinimo
                })
                .ToListAsync();

            return Ok(variantes);
        }

        // GET: api/variantes/stock-bajo
        [HttpGet("stock-bajo")]
        public async Task<IActionResult> GetStockBajo()
        {
            var variantes = await _context.VariantesProducto
                .AsNoTracking()
                .Where(v => v.Stock <= v.StockMinimo)
                .Select(v => new
                {
                    v.IdVariante,
                    Producto = v.Producto!.Nombre,
                    v.Talla,
                    v.Color,
                    v.SKU,
                    v.Stock,
                    v.StockMinimo
                })
                .ToListAsync();

            return Ok(variantes);
        }

        // POST: api/productos/id/variantes
        [HttpPost("/api/productos/{idProducto}/variantes")]
        public async Task<IActionResult> CrearVariante(
            int idProducto,
            VarianteCrearDto dto)
        {
            var producto = await _context.Productos
                .FindAsync(idProducto);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "El producto no existe."
                });
            }

            if (!producto.Estado)
            {
                return BadRequest(new
                {
                    mensaje = "No se pueden agregar variantes a un producto inactivo."
                });
            }

            var sku = dto.SKU.Trim();

            var skuExiste = await _context.VariantesProducto
                .AnyAsync(v => v.SKU == sku);

            if (skuExiste)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe una variante con ese SKU."
                });
            }

            var variante = new VarianteProducto
            {
                Talla = dto.Talla.Trim(),
                Color = dto.Color.Trim(),
                SKU = sku,
                Stock = dto.Stock,
                StockMinimo = dto.StockMinimo,
                IdProducto = idProducto
            };

            _context.VariantesProducto.Add(variante);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVariante),
                new { id = variante.IdVariante },
                new
                {
                    variante.IdVariante,
                    variante.Talla,
                    variante.Color,
                    variante.SKU,
                    variante.Stock,
                    variante.StockMinimo,
                    variante.IdProducto
                });
        }

        // PUT: api/variantes/id
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarVariante(
            int id,
            VarianteActualizarDto dto)
        {
            var variante = await _context.VariantesProducto
                .FindAsync(id);

            if (variante == null)
            {
                return NotFound(new
                {
                    mensaje = "La variante no existe."
                });
            }

            var sku = dto.SKU.Trim();

            var skuExiste = await _context.VariantesProducto
                .AnyAsync(v =>
                    v.SKU == sku &&
                    v.IdVariante != id);

            if (skuExiste)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe otra variante con ese SKU."
                });
            }

            variante.Talla = dto.Talla.Trim();
            variante.Color = dto.Color.Trim();
            variante.SKU = sku;
            variante.StockMinimo = dto.StockMinimo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Variante actualizada correctamente."
            });
        }

        // PUT: api/variantes/id/stock
        [HttpPut("{id}/stock")]
        public async Task<IActionResult> ActualizarStock(
            int id,
            StockActualizarDto dto)
        {
            var variante = await _context.VariantesProducto
                .FindAsync(id);

            if (variante == null)
            {
                return NotFound(new
                {
                    mensaje = "La variante no existe."
                });
            }

            var stockAnterior = variante.Stock;

            variante.Stock = dto.Stock;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Stock actualizado correctamente.",
                stockAnterior,
                stockNuevo = variante.Stock
            });
        }

        // DELETE: api/variantes/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarVariante(int id)
        {
            var variante = await _context.VariantesProducto
                .FindAsync(id);

            if (variante == null)
            {
                return NotFound(new
                {
                    mensaje = "La variante no existe."
                });
            }

            var apareceEnPedidos = await _context.DetallesPedido
                .AnyAsync(d => d.IdVariante == id);

            if (apareceEnPedidos)
            {
                return Conflict(new
                {
                    mensaje =
                        "No se puede eliminar la variante porque aparece en pedidos registrados."
                });
            }

            _context.VariantesProducto.Remove(variante);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}