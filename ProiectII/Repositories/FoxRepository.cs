using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Repositories
{
    public class FoxRepository : GenericRepository<Fox>, IFoxRepository
    {
        public FoxRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Fox>> GetFoxesWithDetailsAsync()
        {
            return await _context.Foxes
                .Include(f => f.Status)
                .Include(f => f.FoxEnclosure)
                .Include(f => f.FirstSeenLocation)
                .Include(f => f.LastSeenLocation)
                .Where(f => !f.IsDeleted) 
                .ToListAsync();
        }

        public async Task<Fox?> GetFoxByIdWithDetailsAsync(uint id)
        {
            return await _context.Foxes
                .Include(f => f.Status)
                .Include(f => f.FoxEnclosure)
                .Include(f => f.FirstSeenLocation)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}