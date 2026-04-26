using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.AuthAccount;
using ProiectII.Interfaces;
using ProiectII.Models;
using System.Security.Claims;

namespace ProiectII.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _fileStorageService;

        public UserController(UserManager<ApplicationUser> userManager, IFileStorageService fileStorageService)
        {
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.BornDate = dto.BirthDate;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { Message = "Profil actualizat cu succes." });
        }

        // 2. Schimbare Parolă (În interiorul contului)
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            // ChangePasswordAsync verifică automat dacă parola veche este corectă
            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { Message = "Parola a fost schimbată." });
        }

        // 3. Încărcare Poză de Profil (Folosind fișier binar)
        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Niciun fișier selectat.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            try
            {
                // Salvăm fișierul folosind serviciul tău profesional
                string relativePath = await _fileStorageService.SaveFileAsync(file, "profiles");

                // Ștergem poza veche de pe disc (opțional, dar recomandat pentru spațiu)
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    _fileStorageService.DeleteFile(user.ProfilePictureUrl);
                }

                // Actualizăm calea în baza de date
                user.ProfilePictureUrl = relativePath;
                await _userManager.UpdateAsync(user);

                return Ok(new { Url = relativePath });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}