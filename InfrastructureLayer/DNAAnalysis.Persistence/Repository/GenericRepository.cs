using Microsoft.EntityFrameworkCore;
using DNAAnalysis.Domain.Contracts;
using DNAAnalysis.Domain.Entities;
using DNAAnalysis.Persistence.Data.DBContexts;
using System.Linq.Expressions;

namespace DNAAnalysis.Persistence.Repository
{
    public class GenericRepository<TEntity, TKey>
        : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        private readonly DNAAnalysisDbContext _dbContext;

        public GenericRepository(DNAAnalysisDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
            => await _dbContext.Set<TEntity>().AddAsync(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbContext.Set<TEntity>().ToListAsync();

        // ✅ الفلترة في SQL مباشرة
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> predicate)
            => await _dbContext.Set<TEntity>()
                               .Where(predicate)
                               .ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id)
            => await _dbContext.Set<TEntity>().FindAsync(id);

        public void Remove(TEntity entity)
            => _dbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity)
            => _dbContext.Set<TEntity>().Update(entity);

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbContext.Set<TEntity>()
                               .FirstOrDefaultAsync(predicate);
    }
}