using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Interfaces;

public interface IConferenciaAppService : IAppService
{
    Task<CfgResultDto> GetFilaAsync(ConferenciaFilaRequest request, CancellationToken cancellationToken = default);
    Task<ConferenciaDetalheResponse?> GetDetalheAsync(ulong recebimentoId, CancellationToken cancellationToken = default);
    Task<ConferenciaDetalheResponse?> IniciarAsync(ulong recebimentoId, IniciarConferenciaRequest request, CancellationToken cancellationToken = default);
    Task<ConferenciaDetalheResponse?> FinalizarAsync(ulong recebimentoId, FinalizarConferenciaRequest request, CancellationToken cancellationToken = default);
}
