using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.DTO.FoxManagement;
using ProiectII.Models;

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

        [Authorize(Roles = "Admin")]
        [HttpPost("fox-statuses")]
        public async Task<IActionResult> CreateStatus([FromBody] CreateStatusDto dto)
        {
            var status = new Status { Name = dto.Name }; // Presupunem ca Entitatea ta e 'Status'
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();
            return Ok(new { status.Id, status.Name });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("fox-statuses/{id}")]
        public async Task<IActionResult> DeleteStatus(uint id)
        {
            // Securitate: Verificăm dacă există vulpi care folosesc acest status
            var inUse = await _context.Foxes.AnyAsync(f => f.StatusId == id);
            if (inUse)
                return BadRequest("Eroare: Acest status este atribuit unor vulpi. Nu poate fi șters.");

            var status = await _context.Statuses.FindAsync(id);
            if (status == null) return NotFound();

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Status șters cu succes." });
        }









    }
}