using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Cfg.Interfaces;

public interface ICfgConsultaAppService : IAppService
{
    Task<CfgResultDto> ProcessQueryAsync(
        string identificador,
        CfgRequestDataDto request,
        CancellationToken cancellationToken = default);
}
