using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record ConferenciaFilaRequest(
    IReadOnlyCollection<CfgFilterFieldDto>? Filter,
    IReadOnlyCollection<CfgFilterFieldDto>? FilterCode,
    int Page = 1,
    int DataCountByPage = 20,
    string? OrderByField = null,
    string? OrderByType = null);
