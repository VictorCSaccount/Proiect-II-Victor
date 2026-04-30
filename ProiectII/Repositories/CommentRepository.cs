using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Models;
using ProiectII.Interfaces;

namespace ProiectII.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetCommentsByFoxIdAsync(uint foxId)
        {
            return await _dbSet
                .AsNoTracking() // Mutat sus, e mai curat
                .Include(c => c.User)
                .Where(c => c.FoxId == foxId && !c.IsDeleted && c.IsVisible)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(string userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.UserId == userId && !c.IsDeleted && c.IsVisible)
                .ToListAsync();
        }

        // CRITIC: Trebuie override pentru a inlocui metoda din GenericRepository
        public override async Task<Comment?> GetByIdAsync(uint id)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}