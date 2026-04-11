using Microsoft.EntityFrameworkCore;
using ProiectII.Models;

namespace ProiectII.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Fox> Foxes { get; set; }

    }
}
