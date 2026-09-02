using ApiSportGYM.DTOs;
using APISportGYM.Data;
using APISportGYM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public CategoriasController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/categorias
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre,
                    c.Descripcion,
                    c.Estado
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/categorias/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .AsNoTracking()
                .Where(c => c.IdCategoria == id)
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre,
                    c.Descripcion,
                    c.Estado
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "La categoría no existe."
                });
            }

            return Ok(categoria);
        }

        // POST: api/categorias
        [HttpPost]
        public async Task<IActionResult> CrearCategoria(
            CategoriaCrearDto dto)
        {
            var nombre = dto.Nombre.Trim();

            var existe = await _context.Categorias
                .AnyAsync(c => c.Nombre == nombre);

            if (existe)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe una categoría con ese nombre."
                });
            }

            var categoria = new Categoria
            {
                Nombre = nombre,
                Descripcion = dto.Descripcion?.Trim(),
                Estado = true
            };

            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                new
                {
                    categoria.IdCategoria,
                    categoria.Nombre,
                    categoria.Descripcion,
                    categoria.Estado
                });
        }

        // PUT: api/categorias/id
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCategoria(
            int id,
            CategoriaActualizarDto dto)
        {
            var categoria =
                await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "La categoría no existe."
                });
            }

            var nombre = dto.Nombre.Trim();

            var existe = await _context.Categorias
                .AnyAsync(c =>
                    c.Nombre == nombre &&
                    c.IdCategoria != id);

            if (existe)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe otra categoría con ese nombre."
                });
            }

            categoria.Nombre = nombre;
            categoria.Descripcion = dto.Descripcion?.Trim();
            categoria.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Categoría actualizada correctamente."
            });
        }

        // DELETE: api/categorias/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria =
                await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "La categoría no existe."
                });
            }

            var tieneProductosActivos =
                await _context.Productos.AnyAsync(p =>
                    p.IdCategoria == id &&
                    p.Estado);

            if (tieneProductosActivos)
            {
                return Conflict(new
                {
                    mensaje =
                        "No se puede desactivar la categoría porque tiene productos activos."
                });
            }

            categoria.Estado = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}