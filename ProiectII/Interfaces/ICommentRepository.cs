using ProiectII.Models;

namespace ProiectII.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<Comment?> GetByIdAsync(uint id);
        Task<IEnumerable<Comment>> GetCommentsByFoxIdAsync(uint foxId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(string userId);
        // AM STERS UpdateAsync. Folosim Update() din IGenericRepository.
    }
}