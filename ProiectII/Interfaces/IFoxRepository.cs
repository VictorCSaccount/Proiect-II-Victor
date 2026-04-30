using ProiectII.Models;

namespace ProiectII.Interfaces
{
    public interface IFoxRepository : IGenericRepository<Fox>
    {
        Task<IEnumerable<Fox>> GetFoxesWithDetailsAsync();
        Task<Fox?> GetFoxByIdWithDetailsAsync(uint id);
    }
}