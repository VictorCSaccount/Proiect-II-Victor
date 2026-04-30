using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        public ReportRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Returneaza toate rapoartele
        public async Task<IEnumerable<Report>> GetAllReportsWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Reporter) // Include datele despre User
                .Include(r => r.Location) // Include coordonatele
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Returneaza doar rapoartele in desfasurare (Pending sau Investigating)
        public async Task<IEnumerable<Report>> GetAllActiveReportsWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.Reporter)
                .Include(r => r.Location)
                // Presupunem ca "Resolved" sau "Closed" nu mai sunt active
                .Where(r => r.ReportStatus == ReportStatus.Pending || r.ReportStatus == ReportStatus.Investigating)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Returneaza un singur raport cu toate datele
        public async Task<Report?> GetByIdWithDetailsAsync(uint id)
        {
            return await _dbSet
                .Include(r => r.Reporter)
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}