using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;

namespace ProiectII.Repositories
{

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Optimizat cu AsNoTracking pentru performanță
        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.AsNoTracking().ToListAsync();

        // Atenție: Această metodă funcționează doar pentru entități cu PK de tip uint
        public async Task<T?> GetByIdAsync(uint id) =>
            await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) =>
            await _dbSet.AddAsync(entity);

        // Update și Delete rămân sincrone 
        public void Update(T entity) =>
            _dbSet.Update(entity);

        public void Delete(T entity) =>
            _dbSet.Remove(entity);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}