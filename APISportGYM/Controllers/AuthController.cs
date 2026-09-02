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
    public class AuthController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public AuthController(SportFitDbContext context)
        {
            _context = context;
        }


        // POST: api/auth/registro
        [HttpPost("registro")]
        public async Task<IActionResult> Registro(RegistroDto dto)
        {
            var correo = dto.Correo
                .Trim()
                .ToLower();

            var correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo == correo);

            if (correoExiste)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe una cuenta con ese correo."
                });
            }

            var rolCliente = await _context.Roles
                .FirstOrDefaultAsync(r => r.Nombre == "Cliente");

            if (rolCliente == null)
            {
                return StatusCode(500, new
                {
                    mensaje = "No se encontró el rol Cliente en la base de datos."
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
                IdRol = rolCliente.IdRol
            };

            var passwordHasher = new PasswordHasher<Usuario>();

            usuario.Contrasena = passwordHasher.HashPassword(
                usuario,
                dto.Contrasena
            );

            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            return Created("", new
            {
                mensaje = "Cuenta creada correctamente.",

                usuario = new
                {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Estado,

                    rol = "Cliente"
                }
            });
        }


        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var correo = dto.Correo
                .Trim()
                .ToLower();

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }

            if (!usuario.Estado)
            {
                return Unauthorized(new
                {
                    mensaje = "La cuenta se encuentra desactivada."
                });
            }

            var passwordHasher =
                new PasswordHasher<Usuario>();

            var resultado = passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Contrasena,
                dto.Contrasena
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }

            return Ok(new
            {
                mensaje = "Inicio de sesión correcto.",

                usuario = new
                {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Correo,
                    usuario.Telefono,

                    rol = usuario.Rol!.Nombre
                }
            });
        }
    }
}