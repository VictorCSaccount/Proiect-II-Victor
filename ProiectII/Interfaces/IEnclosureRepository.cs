using ProiectII.Models;

namespace ProiectII.Interfaces
{
    public interface IEnclosureRepository : IGenericRepository<Enclosure>
    {
        Task<Enclosure?> GetEnclosureWithPointsAsync(uint id);
    }
}
