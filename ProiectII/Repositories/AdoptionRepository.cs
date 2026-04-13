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
            return await _dbSet
                .AsNoTracking() // Optimizare performanță
                .Include(a => a.User) // Detalii despre cel care adoptă
                .Include(a => a.Fox)
                    .ThenInclude(f => f.Status) // Critic: să vedem dacă vulpea e încă "Healthy" sau deja "Adopted"
                .OrderByDescending(a => a.RequestDate) // Adopțiile noi ar trebui să fie primele
                .ToListAsync();
        }
    }
}
