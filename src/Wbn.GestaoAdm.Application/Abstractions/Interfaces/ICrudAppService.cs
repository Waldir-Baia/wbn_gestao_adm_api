namespace Wbn.GestaoAdm.Application.Abstractions.Interfaces;

public interface ICrudAppService<TResponse, in TKey, in TCreateRequest, in TUpdateRequest> : IAppService
{
    Task<TResponse?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TResponse> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);
    Task<TResponse?> UpdateAsync(TKey id, TUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}
