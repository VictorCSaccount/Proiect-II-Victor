using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Repositories
{
    public class AdoptionRepository : GenericRepository<Adoption>, IAdoptionRepository
    {

        public AdoptionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Adoption>> GetAllWithDetailsAsync()
        {
            return await _context.Adoptions
                .AsNoTracking()
                .Include(a => a.Fox)
                .Include(a => a.User)
                .OrderByDescending(a => a.RequestDate)
                .ToListAsync();
        }

        // Am adaugat metoda necesara pentru extragerea unei singure inregistrari
        public async Task<Adoption?> GetByIdWithDetailsAsync(uint id)
        {
            return await _context.Adoptions
                .Include(a => a.Fox)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}