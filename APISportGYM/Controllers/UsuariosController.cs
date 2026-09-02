using ApiSportGYM.DTOs;
using APISportGYM.Data;
using APISportGYM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public UsuariosController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios(
            [FromQuery] string? rol,
            [FromQuery] bool soloActivos = false)
        {
            var consulta = _context.Usuarios
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(rol))
            {
                consulta = consulta.Where(u =>
                    u.Rol != null &&
                    u.Rol.Nombre == rol);
            }

            if (soloActivos)
            {
                consulta = consulta.Where(u => u.Estado);
            }

            var usuarios = await consulta
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nombre,
                    u.Apellido,
                    u.Correo,
                    u.Telefono,
                    u.Estado,
                    u.FechaRegistro,

                    Rol = new
                    {
                        u.Rol!.IdRol,
                        u.Rol.Nombre
                    }
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/usuarios/id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.IdUsuario == id)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nombre,
                    u.Apellido,
                    u.Correo,
                    u.Telefono,
                    u.Estado,
                    u.FechaRegistro,

                    Rol = new
                    {
                        u.Rol!.IdRol,
                        u.Rol.Nombre
                    }
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe."
                });
            }

            return Ok(usuario);
        }

        // GET: api/usuarios/rol/Repartidor o Cliente
        [HttpGet("rol/{rol}")]
        public async Task<IActionResult> GetUsuariosPorRol(string rol)
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Where(u =>
                    u.Rol != null &&
                    u.Rol.Nombre == rol &&
                    u.Estado)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nombre,
                    u.Apellido,
                    u.Correo,
                    u.Telefono,

                    Rol = u.Rol!.Nombre
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<IActionResult> CrearUsuario(
            UsuarioCrearDto dto)
        {
            var correo = dto.Correo.Trim().ToLower();

            var correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo == correo);

            if (correoExiste)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe un usuario con ese correo."
                });
            }

            var rol = await _context.Roles
                .FindAsync(dto.IdRol);

            if (rol == null)
            {
                return BadRequest(new
                {
                    mensaje = "El rol indicado no existe."
                });
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Correo = correo,
                Telefono = dto.Telefono?.Trim(),
                Estado = true,
                FechaRegistro = DateTime.Now,
                IdRol = dto.IdRol
            };

            var passwordHasher = new PasswordHasher<Usuario>();

            usuario.Contrasena = passwordHasher.HashPassword(
                usuario,
                dto.Contrasena
            );

            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.IdUsuario },
                new
                {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Estado,
                    usuario.FechaRegistro,
                    usuario.IdRol
                });
        }

        // PUT: api/usuarios/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarUsuario(
            int id,
            UsuarioActualizarDto dto)
        {
            var usuario = await _context.Usuarios
                .FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe."
                });
            }

            var correo = dto.Correo.Trim().ToLower();

            var correoExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.Correo == correo &&
                    u.IdUsuario != id);

            if (correoExiste)
            {
                return Conflict(new
                {
                    mensaje = "Ese correo ya pertenece a otro usuario."
                });
            }

            var rol = await _context.Roles
                .FindAsync(dto.IdRol);

            if (rol == null)
            {
                return BadRequest(new
                {
                    mensaje = "El rol indicado no existe."
                });
            }

            usuario.Nombre = dto.Nombre.Trim();
            usuario.Apellido = dto.Apellido.Trim();
            usuario.Correo = correo;
            usuario.Telefono = dto.Telefono?.Trim();
            usuario.Estado = dto.Estado;
            usuario.IdRol = dto.IdRol;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario actualizado correctamente."
            });
        }

        // DELETE: api/usuarios/id
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe."
                });
            }
            usuario.Estado = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}