using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

public sealed record RecebimentosListaRequest(
    IReadOnlyCollection<CfgFilterFieldDto>? Filter,
    IReadOnlyCollection<CfgFilterFieldDto>? FilterCode,
    int Page = 1,
    int DataCountByPage = 20,
    string? OrderByField = null,
    string? OrderByType = null);
