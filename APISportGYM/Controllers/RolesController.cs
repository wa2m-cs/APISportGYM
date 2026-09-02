using APISportGYM.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiSportGYM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly SportFitDbContext _context;

        public RolesController(SportFitDbContext context)
        {
            _context = context;
        }

        // GET: api/roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .Select(r => new
                {
                    r.IdRol,
                    r.Nombre,
                    r.Descripcion
                })
                .ToListAsync();

            return Ok(roles);
        }
    }
}