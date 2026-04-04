using System.Linq.Expressions;
using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Common.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T> Create(T obj);
    Task<List<T>> CreateRange(List<T> obj);
    Task<List<T>> UpdateRange(List<T> obj);
    Task<T> Update(T obj);
    Task Remove(ulong id);
    Task<T?> Get(ulong id);
    Task<List<T>> GetAll();
    Task<List<T>> GetAll(Expression<Func<T, bool>> predicate);
    Task<bool> RecordExists(ulong id);
    Task UpdateCampo(T obj, ulong id);
}
