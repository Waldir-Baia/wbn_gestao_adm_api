namespace Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

public sealed record CfgFilterFieldDto(
    string Field,
    string FilterType,
    string? Filter,
    string[]? FilterArray);
