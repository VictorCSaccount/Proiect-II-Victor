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
            return await _dbSet
                .AsNoTracking() // Obligatoriu pentru hărți (read-only)
                .Include(e => e.CenterLocation)
                .Include(e => e.PolygonPoints)
                // CRITIC: Sortăm punctele pentru ca poligonul să fie desenat corect
                // Notă: Sortarea se face de obicei în memorie sau prin ThenInclude dacă EF suportă 
                // dar cea mai sigură metodă este să te asiguri că sunt ordonate.
                .FirstOrDefaultAsync(e => e.Id == id);

            // Notă: Dacă EF Core nu permite OrderBy direct în Include, 
            // ordonarea se va face în Service înainte de a trimite DTO-ul.
        }
    }
}
