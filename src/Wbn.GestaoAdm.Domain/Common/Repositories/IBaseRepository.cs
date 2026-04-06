using System.Linq.Expressions;
using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Common.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T> Create(T obj, CancellationToken cancellationToken = default);
    Task<List<T>> CreateRange(List<T> obj, CancellationToken cancellationToken = default);
    Task<List<T>> UpdateRange(List<T> obj, CancellationToken cancellationToken = default);
    Task<T> Update(T obj, CancellationToken cancellationToken = default);
    Task Remove(ulong id, CancellationToken cancellationToken = default);
    Task<T?> Get(ulong id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAll(CancellationToken cancellationToken = default);
    Task<List<T>> GetAll(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> RecordExists(ulong id, CancellationToken cancellationToken = default);
}
