using ProiectII.Models;

namespace ProiectII.Interfaces
{
    public interface IFoxRepository : IGenericRepository<Fox>
    {
        // Metoda specifică pentru a aduce vulpea cu Status, Locație și Țarc
        Task<IEnumerable<Fox>> GetFoxesWithDetailsAsync();
        Task<Fox?> GetFoxByIdWithDetailsAsync(uint id);
    }
}