using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SecurityLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SecurityLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = await _context.SecurityLogs
                .Include(s => s.User)
                .OrderByDescending(s => s.Timestamp)
                .Take(100)
                .Select(s => new {
                    s.Id,
                    Email = s.User != null ? s.User.Email : "Sistem",
                    s.Action,
                    s.Details,
                    s.Timestamp
                })
                .ToListAsync();

            return Ok(logs);
        }
    }
}