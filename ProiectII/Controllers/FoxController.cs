using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.DTO.FoxManagement;
using ProiectII.Interfaces;
using ProiectII.Models;
using ProiectII.Services.CoreDomain;

namespace ProiectII.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
//public class FoxController(IFoxService foxService, IMapper mapper) : ControllerBase
//{

//public class FoxController(IFoxService foxService, IMapper mapper, ApplicationDbContext _context) : ControllerBase
//{

public class FoxController(
    IFoxService foxService,
    IMapper mapper,
    ApplicationDbContext _context // Asigură-te că numele este EXACT _context
) : ControllerBase
{


    [HttpGet]
    public async Task<IActionResult> GetFoxes()
    {
        var foxes = await foxService.GetAllFoxesAsync();

        // Dacă nu e admin sau angajat, ascundem vulpile adoptate
        if (!User.IsInRole("Admin") && !User.IsInRole("Employee"))
        {
            // Modifică 'Status' în 'StatusName' dacă așa e în DTO-ul tău
            foxes = foxes.Where(f => f.StatusName != "Adopted").ToList();
        }

        return Ok(foxes);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetFox(uint id)
    {
        var result = await foxService.GetFoxByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPost]
    public async Task<IActionResult> CreateFox([FromForm] CreateFoxDto dto)
    {
        var result = await foxService.CreateFoxAsync(dto);
        return Ok(new { Message = "Vulpe creată cu succes", Data = result });
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFox(uint id, [FromBody] UpdateFoxDto dto)
    {
        var success = await foxService.UpdateFoxAsync(id, dto);
        return success ? Ok(new { Message = "Date actualizate." }) : NotFound();
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateFoxStatus(uint id, [FromBody] UpdateFoxStatusDto dto)
    {
        var success = await foxService.UpdateFoxStatusAsync(id, dto);
        return success ? Ok(new { Message = "Status actualizat." }) : NotFound();
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ArchiveFox(uint id)
    {
        var success = await foxService.ArchiveFoxAsync(id);
        return success ? Ok(new { Message = "Vulpe arhivată." }) : NotFound();
    }






    [AllowAnonymous]
    [HttpGet("map-markers")]
    public async Task<IActionResult> GetMapMarkers()
    {
        var activeMarkers = await foxService.GetMapMarkersAsync();

        // Vizitatorii anonimi (Identity == null) sau Userii simpli nu văd adopțiile
        if (User.Identity == null || (!User.IsInRole("Admin") && !User.IsInRole("Employee")))
        {
            activeMarkers = activeMarkers.Where(m => m.StatusName != "Adopted").ToList();
        }

        return Ok(activeMarkers);
    }


    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/location")]
    public async Task<IActionResult> UpdateLocation(uint id, [FromBody] UpdateFoxLocationDto dto)
    {
        // 1. Căutăm vulpea folosind numele corect: LastSeenLocation
        var fox = await _context.Foxes
            .Include(f => f.LastSeenLocation)
                .ThenInclude(l => l.Coordinate)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fox == null) return NotFound("Vulpea nu a fost găsită.");

        // 2. Verificăm LastSeenLocation (nu Location!)
        if (fox.LastSeenLocation != null && fox.LastSeenLocation.Coordinate != null)
        {
            fox.LastSeenLocation.Coordinate.Latitude = dto.Latitude;
            fox.LastSeenLocation.Coordinate.Longitude = dto.Longitude;
        }
        else
        {
            // 3. Creăm o locație nouă dacă nu există
            fox.LastSeenLocation = new Location
            {
                Name = "Last Known Location",
                Coordinate = new Coordinate { Latitude = dto.Latitude, Longitude = dto.Longitude }
            };
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Locația vulpii a fost actualizată." });
    }





}