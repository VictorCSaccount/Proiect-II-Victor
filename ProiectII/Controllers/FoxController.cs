using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data; // Asigură-te că ai namespace-ul corect pentru DbContext
using ProiectII.Models;

namespace ProiectII.Controllers
{
    [ApiController] // Definește clasa ca API
    [Route("api/[controller]")] // Ruta va fi: api/fox
    public class FoxController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FoxController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/fox
        // Aceasta este metoda pe care o va apela colegul tău pentru hartă
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fox>>> GetFoxes()
        {
            return await _context.Foxes.ToListAsync();
        }

        // POST: api/fox
        // Folosește asta în Swagger/Postman pentru a adăuga date
        [HttpPost]
        public async Task<ActionResult<Fox>> PostFox(Fox fox)
        {
            _context.Foxes.Add(fox);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFoxes), new { id = fox.Id }, fox);
        }
    }
}