using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Interfaces;

public interface IMobileRecebimentoAppService
{
    Task<IReadOnlyCollection<MobileRecebimentoSummaryResponse>> GetMeusEnviosAsync(
        ulong usuarioId,
        CancellationToken cancellationToken = default);

    Task<MobileRecebimentoDetalheResponse?> GetDetalheAsync(
        ulong id,
        ulong usuarioId,
        CancellationToken cancellationToken = default);

    Task<MobileRecebimentoDetalheResponse> CreateAsync(
        MobileCreateRecebimentoRequest request,
        CancellationToken cancellationToken = default);

    Task<MobileDocumentoResponse> EnviarDocumentoAsync(
        MobileEnviarDocumentoRequest request,
        CancellationToken cancellationToken = default);
}
