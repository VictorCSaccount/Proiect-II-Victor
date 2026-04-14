using ProiectII.Models;
namespace ProiectII.Interfaces
{
    public interface ICommentRepository: IGenericRepository<Comment>
    {

        Task<IEnumerable<Comment>> GetCommentsByFoxIdAsync(uint foxId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(string userId);
        Task UpdateAsync(Comment comment);

       
    }
}
