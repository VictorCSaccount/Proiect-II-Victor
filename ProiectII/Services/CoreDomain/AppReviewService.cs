using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.DTO;
using ProiectII.DTO.Reviews;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Services.CoreDomain;

public class AppReviewService(ApplicationDbContext _context) : IAppReviewService
{
    public async Task<IEnumerable<object>> GetAllReviewsAsync()
    {
        return await _context.AppReviews
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new {
                r.Id,
                UserName = r.User != null ? r.User.FirstName + " " + r.User.LastName : "Anonim",
                r.Rating,
                r.Comment,
                r.CreatedAt
            }).ToListAsync();
    }

    public async Task CreateReviewAsync(CreateAppReviewDto dto, string? userId)
    {
        var review = new AppReview { UserId = userId, Rating = dto.Rating, Comment = dto.Comment };
        _context.AppReviews.Add(review);
        await _context.SaveChangesAsync();
    }
}