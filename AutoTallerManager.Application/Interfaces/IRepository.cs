using System.Linq.Expressions;

namespace AutoTallerManager.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIntIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<(IEnumerable<T> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize);
    IQueryable<T> Find(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
