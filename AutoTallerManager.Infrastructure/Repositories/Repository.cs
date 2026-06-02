using System.Linq.Expressions;
using AutoTallerManager.Application.Interfaces;
using AutoTallerManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AutoTallerDbContext Context;

    public Repository(AutoTallerDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByIntIdAsync(int id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return Context.Set<T>().Where(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().AnyAsync(predicate);
    }

    public async Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector)
    {
        return await Context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public async Task AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        Context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
    }

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var query = Context.Set<T>().AsNoTracking();
        var totalCount = await query.CountAsync();

        var actualPage = pageNumber < 1 ? 1 : pageNumber;
        var actualSize = pageSize < 1 ? 10 : pageSize;

        var items = await query
            .Skip((actualPage - 1) * actualSize)
            .Take(actualSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
