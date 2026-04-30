using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.AdminSystem;
using ProiectII.Models;

namespace ProiectII.Controllers
{




    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Protecția obligatorie
    public class AdminUserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateUserRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("Utilizatorul nu a fost găsit.");

            // Verificare de siguranță: să nu își scoată singur rolul de Admin
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId && dto.NewRole != "Admin")
                return BadRequest("Nu îți poți retrage singur rolul de Admin.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            var addResult = await _userManager.AddToRoleAsync(user, dto.NewRole);
            if (!addResult.Succeeded)
                return BadRequest(addResult.Errors);

            return Ok(new { Message = $"Rolul a fost actualizat în {dto.NewRole}." });
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UserStatusUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("Utilizatorul nu a fost găsit.");

            // Validare logică: dacă îl dezactivezi, trebuie să zici de ce
            if (!dto.IsActive && string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("Este necesar un motiv pentru dezactivarea contului.");

            user.IsActive = dto.IsActive;
            user.DeactivationReason = dto.IsActive ? null : dto.Reason;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = dto.IsActive ? "Cont activat." : "Cont dezactivat." });
        }
    }











}