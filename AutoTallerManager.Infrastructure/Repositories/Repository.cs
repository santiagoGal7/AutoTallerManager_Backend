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

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return Context.Set<T>().Where(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().AnyAsync(predicate);
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
}
