namespace Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

public sealed record CfgRequestDataDto(
    IReadOnlyCollection<CfgFilterFieldDto>? Filter,
    IReadOnlyCollection<CfgFilterFieldDto>? FilterCode,
    int Page,
    int DataCountByPage,
    string? OrderByField,
    string? OrderByType);
