using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.DTO;

namespace ProiectII.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
// Primary Constructor - injecteaza contextul direct in linia clasei
public class DashboardController(ApplicationDbContext _context) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        // Preluam datele asincron pentru a nu bloca thread-ul
        var stats = new DashboardStatsDto
        {
            TotalFoxes = await _context.Foxes.CountAsync(f => !f.IsDeleted),

            // Cast la (int) daca statusul tau in baza de date este Enum sau uint
            PendingAdoptions = await _context.Adoptions
                .CountAsync(a => (int)a.AdoptionStatus == 1),

            ActiveReports = await _context.Reports
                .CountAsync(r => (int)r.ReportStatus != 3),

            TotalUsers = await _context.Users.CountAsync()
        };

        // Activitati recente: ultimele 5 rapoarte
        stats.RecentActivities = await _context.Reports
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new RecentActivityDto
            {
                        Title = "Raport Nou",
                        Description = r.Description.Length > 50
                ? r.Description.Substring(0, 50) + "..."
                : r.Description,
                        Date = r.CreatedAt
                    })
        .Take(5)
        .ToListAsync();

        return Ok(stats);
    }
}