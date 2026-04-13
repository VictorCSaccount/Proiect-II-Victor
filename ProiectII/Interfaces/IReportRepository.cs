using ProiectII.Models;

namespace ProiectII.Interfaces
{
    public interface IReportRepository : IGenericRepository<Report>
    {
        Task<IEnumerable<Report>> GetAllReportsWithDetailsAsync();
    }
}
