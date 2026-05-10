using ProiectII.DTO;
using ProiectII.DTO.Reviews;
namespace ProiectII.Interfaces;

public interface IAppReviewService
{
    Task<IEnumerable<object>> GetAllReviewsAsync();
    Task CreateReviewAsync(CreateAppReviewDto dto, string? userId);
}