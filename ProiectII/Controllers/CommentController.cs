using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.CommentsReport;
using ProiectII.Interfaces;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Toti trebuie sa fie logati
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // --- ZONA PUBLICĂ (Utilizatori normali) ---

        [HttpGet("fox/{foxId}")]
        [AllowAnonymous] // Permitem vizualizarea comentariilor chiar si vizitatorilor nelogati
        public async Task<IActionResult> GetCommentsForFox(uint foxId)
        {
            var comments = await _commentService.GetCommentsByFoxIdAsync(foxId);
            return Ok(comments);
        }

        [HttpGet("my-comments")]
        public async Task<IActionResult> GetMyComments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var comments = await _commentService.GetCommentsByUserIdAsync(userId);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _commentService.CreateCommentAsync(dto, userId);
            return Ok(new { Message = "Comentariul a fost adăugat.", Data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditComment(uint id, [FromBody] UpdateCommentDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var success = await _commentService.EditCommentAsync(id, dto, userId);
            if (!success) return BadRequest("Nu poți edita acest comentariu sau nu există.");

            return Ok(new { Message = "Comentariul a fost editat." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(uint id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var success = await _commentService.DeleteCommentAsync(id, userId);
            if (!success) return BadRequest("Nu poți șterge acest comentariu sau nu există.");

            return Ok(new { Message = "Comentariul a fost șters." });
        }


        // --- ZONA DE ADMINISTRARE (Admin / Employee) ---

        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}/hide")]
        public async Task<IActionResult> HideComment(uint id)
        {
            var success = await _commentService.AdminHideComment(id);
            if (!success) return NotFound();
            return Ok(new { Message = "Comentariul a fost ascuns publicului." });
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}/unhide")]
        public async Task<IActionResult> UnhideComment(uint id)
        {
            var success = await _commentService.AdminUnhideComment(id);
            if (!success) return NotFound();
            return Ok(new { Message = "Comentariul este din nou vizibil." });
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete("{id}/admin")]
        public async Task<IActionResult> AdminDeleteComment(uint id)
        {
            var success = await _commentService.AdminDeleteCommentAsync(id);
            if (!success) return NotFound();
            return Ok(new { Message = "Comentariul a fost șters de către administrator." });
        }
    }
}