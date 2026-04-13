using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Report>> GetAllReportsWithDetailsAsync()
        {
            return await _context.Reports
                .Include(r => r.Reporter) 
                .Include(r => r.Location) 
                .OrderByDescending(r => r.Id) 
                .ToListAsync();
        }
    }
}
