namespace Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

public sealed record CfgResultDto(
    string Data,
    string PrimaryKey,
    string? SearchIdentificator,
    string? SearchDescription,
    string? SearchAlternativeCode,
    IReadOnlyCollection<SubCfgCampoDto> SubCfgs);
