using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.DTO;
using ProiectII.DTO.Reviews;
using ProiectII.Models;
using System.Security.Claims;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppReviewController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _context.AppReviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new {
                    r.Id,
                    UserName = r.User != null ? r.User.FirstName + " " + r.User.LastName : "Anonim",
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppReviewDto dto)
        {
            string? currentUserId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var review = new AppReview
            {
                UserId = currentUserId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.AppReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Review adăugat cu succes." });
        }
    }
}