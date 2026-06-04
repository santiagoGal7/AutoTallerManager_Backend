using System.Collections.Generic;
using System.Threading.Tasks;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> CompleteAsync();
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<(IEnumerable<OrdenServicio> Items, int TotalCount)> GetOrdenesUsuarioPaginadoAsync(int usuarioId, int pageNumber, int pageSize);
}
