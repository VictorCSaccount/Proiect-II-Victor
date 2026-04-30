using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferenceDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReferenceDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Returneaza statusurile pentru Vulpi (ex: Healthy, In Treatment, Adopted)
        [HttpGet("fox-statuses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFoxStatuses()
        {
            var statuses = await _context.Statuses
                .AsNoTracking()
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();

            return Ok(statuses);
        }

        // Returneaza locatiile interne (Tarcurile)
        [HttpGet("enclosures")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEnclosures()
        {
            var enclosures = await _context.Enclosures
                .AsNoTracking()
                .Select(e => new { e.Id, e.Name})
                .ToListAsync();

            return Ok(enclosures);
        }

        // Daca ai un tabel separat in DB pentru ReportStatuses, il adaugi aici. 
        // Daca e doar un Enum in C#, Frontend-ul il va mapa local.
    }
}