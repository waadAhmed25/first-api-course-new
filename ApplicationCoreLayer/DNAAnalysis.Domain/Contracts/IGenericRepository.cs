using DNAAnalysis.Domain.Entities;
using System.Linq.Expressions;

namespace DNAAnalysis.Domain.Contracts
{
    public interface IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetAllAsync();

        // ✅ Overload جديد للفلترة في SQL
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity?> GetByIdAsync(TKey id);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Remove(TEntity entity);
    }
}