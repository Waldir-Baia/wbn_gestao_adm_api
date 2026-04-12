using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Interfaces;

public interface IRecebimentoAppService : IAppService
{
    Task<RecebimentoResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<RecebimentoResponse> CreateAsync(CreateRecebimentoRequest request, CancellationToken cancellationToken = default);
    Task<RecebimentoResponse?> UpdateStatusAsync(
        ulong id,
        UpdateRecebimentoStatusRequest request,
        CancellationToken cancellationToken = default);
}
