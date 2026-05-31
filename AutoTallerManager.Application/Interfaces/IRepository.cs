using System.Linq.Expressions;

namespace AutoTallerManager.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
