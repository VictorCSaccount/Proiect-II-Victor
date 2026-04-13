using ProiectII.Models;

namespace ProiectII.Interfaces
{
    public interface IAdoptionRepository : IGenericRepository<Adoption>
    {
        Task<IEnumerable<Adoption>> GetAdoptionsWithDetailsAsync();
    }
}
