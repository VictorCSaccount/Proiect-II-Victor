using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.AdoptionProcess;
using ProiectII.Interfaces;
using ProiectII.Services.CoreDomain;
using System.Security.Claims;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdoptionController : ControllerBase
    {
        private readonly IAdoptionService _adoptionService;

        public AdoptionController(IAdoptionService adoptionService)
        {
            _adoptionService = adoptionService;
        }

        // 1. User aplica pentru o vulpe
        [HttpPost]
        public async Task<IActionResult> ApplyForAdoption([FromBody] AdoptionRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _adoptionService.CreateAdoptionRequestAsync(userId, dto);
            return Ok(new { Message = "Cererea a fost înregistrată.", Data = result });
        }

        // 2. User isi vizualizeaza propriile cereri
        [HttpGet("my-applications")]
        public async Task<IActionResult> GetMyAdoptions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _adoptionService.GetUserAdoptionsAsync(userId);
            return Ok(result);
        }

        // 3. Admin vizualizeaza toate cererile
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAdoptions()
        {
            var result = await _adoptionService.GetAllAdoptionsAsync();
            return Ok(result);
        }

        // 4. Admin proceseaza o cerere (Aproba/Respinge)
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("process")]
        public async Task<IActionResult> ProcessAdoption([FromBody] ProcessAdoptionDto dto)
        {
            var success = await _adoptionService.ProcessAdoptionAsync(dto);
            if (!success) return NotFound("Cererea de adopție nu există.");

            return Ok(new { Message = "Cererea a fost procesată cu succes." });
        }
    }
}