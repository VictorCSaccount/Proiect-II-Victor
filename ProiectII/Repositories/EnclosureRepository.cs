using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Repositories
{
    public class EnclosureRepository : GenericRepository<Enclosure>, IEnclosureRepository
    {
        public EnclosureRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Enclosure?> GetEnclosureWithPointsAsync(uint id)
        {
            return await _context.Enclosures
                .Include(e => e.PolygonPoints)
                .Include(e => e.CenterLocation)      
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
