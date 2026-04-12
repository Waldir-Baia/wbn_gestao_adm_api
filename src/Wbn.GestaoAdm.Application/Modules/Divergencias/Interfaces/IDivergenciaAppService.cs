using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Divergencias.Interfaces;

public interface IDivergenciaAppService : IAppService
{
    Task<DivergenciaResponse> CreateAsync(CreateDivergenciaRequest request, CancellationToken cancellationToken = default);
    Task<DivergenciaResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<DivergenciaResponse?> UpdateStatusAsync(ulong id, UpdateDivergenciaStatusRequest request, CancellationToken cancellationToken = default);
    Task<DivergenciaResponse?> ResolverAsync(ulong id, ResolverDivergenciaRequest request, CancellationToken cancellationToken = default);
}
