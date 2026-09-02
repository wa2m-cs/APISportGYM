using ApiSportGYM.DTOs;
using APISportGYM.Data;
using APISportGYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public ProductosController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<IActionResult> GetProductos(
            [FromQuery] string? buscar,
            [FromQuery] int? categoriaId,
            [FromQuery] bool soloActivos = false)
        {
            var consulta = _context.Productos
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                consulta = consulta.Where(p =>
                    p.Nombre.Contains(buscar) ||
                    (p.Marca != null &&
                     p.Marca.Contains(buscar)));
            }

            if (categoriaId.HasValue)
            {
                consulta = consulta.Where(p =>
                    p.IdCategoria == categoriaId.Value);
            }

            if (soloActivos)
            {
                consulta = consulta.Where(p => p.Estado);
            }

            var productos = await consulta
                .Select(p => new
                {
                    p.IdProducto,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.Marca,
                    p.Genero,
                    p.Imagen,
                    p.Estado,

                    Categoria = new
                    {
                        p.Categoria!.IdCategoria,
                        p.Categoria.Nombre
                    },

                    Variantes = p.Variantes.Select(v => new
                    {
                        v.IdVariante,
                        v.Talla,
                        v.Color,
                        v.SKU,
                        v.Stock,
                        v.StockMinimo
                    })
                })
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Where(p => p.IdProducto == id)
                .Select(p => new
                {
                    p.IdProducto,
                    p.Nombre,
                    p.Descripcion,
                    p.Precio,
                    p.Marca,
                    p.Genero,
                    p.Imagen,
                    p.Estado,

                    Categoria = new
                    {
                        p.Categoria!.IdCategoria,
                        p.Categoria.Nombre
                    },

                    Variantes = p.Variantes.Select(v => new
                    {
                        v.IdVariante,
                        v.Talla,
                        v.Color,
                        v.SKU,
                        v.Stock,
                        v.StockMinimo
                    })
                })
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "El producto no existe."
                });
            }

            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<IActionResult> CrearProducto(
            ProductoCrearDto dto)
        {
            var categoria =
                await _context.Categorias.FindAsync(dto.IdCategoria);

            if (categoria == null || !categoria.Estado)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La categoría indicada no existe o está inactiva."
                });
            }

            var producto = new Producto
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion?.Trim(),
                Precio = dto.Precio,
                Marca = dto.Marca?.Trim(),
                Genero = dto.Genero,
                Imagen = dto.Imagen?.Trim(),
                Estado = true,
                IdCategoria = dto.IdCategoria
            };

            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.IdProducto },
                new
                {
                    producto.IdProducto,
                    producto.Nombre,
                    producto.Descripcion,
                    producto.Precio,
                    producto.Marca,
                    producto.Genero,
                    producto.Imagen,
                    producto.Estado,
                    producto.IdCategoria
                });
        }

        // PUT: api/productos/id
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(
            int id,
            ProductoActualizarDto dto)
        {
            var producto =
                await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "El producto no existe."
                });
            }

            var categoria =
                await _context.Categorias.FindAsync(dto.IdCategoria);

            if (categoria == null || !categoria.Estado)
            {
                return BadRequest(new
                {
                    mensaje =
                        "La categoría indicada no existe o está inactiva."
                });
            }

            producto.Nombre = dto.Nombre.Trim();
            producto.Descripcion = dto.Descripcion?.Trim();
            producto.Precio = dto.Precio;
            producto.Marca = dto.Marca?.Trim();
            producto.Genero = dto.Genero;
            producto.Imagen = dto.Imagen?.Trim();
            producto.Estado = dto.Estado;
            producto.IdCategoria = dto.IdCategoria;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Producto actualizado correctamente."
            });
        }

        // DELETE: api/productos/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto =
                await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(new
                {
                    mensaje = "El producto no existe."
                });
            }

            producto.Estado = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}