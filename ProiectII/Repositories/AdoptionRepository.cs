using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Repositories
{
    public class AdoptionRepository : GenericRepository<Adoption>, IAdoptionRepository
    {
        public AdoptionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Adoption>> GetAdoptionsWithDetailsAsync()
        {
            return await _context.Adoptions
                .Include(a => a.Fox)
                .Include(a => a.User) // User ul ce  af acut cererea de adoptie
                .ToListAsync();
        }
    }
}
