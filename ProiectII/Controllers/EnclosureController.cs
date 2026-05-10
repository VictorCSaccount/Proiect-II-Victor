using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data; // Asigură-te că ai namespace-ul corect pentru ApplicationDbContext
using ProiectII.DTO.MapTracking;
using ProiectII.Models;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnclosureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Injectarea bazei de date (Obligatoriu)
        public EnclosureController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. CREARE ȚARC (Doar pentru Admin)
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost] // Ruta va fi POST /api/Enclosure
        public async Task<IActionResult> CreateEnclosure([FromBody] CreateEnclosureDto dto)
        {
            var enclosure = new Enclosure
            {
                Name = dto.Name,
                Description = dto.Description,
                ColorMaskHex = dto.ColorMaskHex,
                Opacity = dto.Opacity,
                CenterLocationId = dto.CenterLocationId
            };

            _context.Enclosures.Add(enclosure);
            await _context.SaveChangesAsync(); // Salvăm ca să primim enclosure.Id

            // Adăugăm punctele poligonului dacă există
            if (dto.Points != null && dto.Points.Any())
            {
                foreach (var point in dto.Points)
                {
                    _context.EnclosurePoints.Add(new EnclosurePoint
                    {
                        EnclosureId = enclosure.Id,
                        Coordinate = new Coordinate
                        {
                            Latitude = point.Latitude,
                            Longitude = point.Longitude
                        },
                        DrawOrder = point.Order
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Țarc și poligon create cu succes." });
        }

        // ==========================================
        // 2. AFIȘARE POLIGOANE (Pentru Hartă - Public)
        // ==========================================
        [AllowAnonymous]
        [HttpGet("polygons")] // Ruta va fi GET /api/Enclosure/polygons
        public async Task<IActionResult> GetPolygons()
        {
            var data = await _context.Enclosures
                .Include(e => e.PolygonPoints) // Corectat: Folosim numele real din model
                .ThenInclude(p => p.Coordinate) // Trebuie să includem și coordonatele
                .Select(e => new {
                    e.Id,
                    e.Name,
                    e.ColorMaskHex,
                    e.Opacity,
                    // Corectat: Folosim PolygonPoints și navigăm prin Coordinate
                    Points = e.PolygonPoints.OrderBy(p => p.DrawOrder)
                                     .Select(p => new {
                                         Latitude = p.Coordinate.Latitude,
                                         Longitude = p.Coordinate.Longitude
                                     })
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}