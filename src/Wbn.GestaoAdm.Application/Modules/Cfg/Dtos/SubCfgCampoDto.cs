namespace Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;

public sealed record SubCfgCampoDto(
    string Campo,
    string Descricao,
    string TipoDados,
    int OrdemCampo,
    bool PermitirFiltro,
    bool Visivel,
    string CampoBusca,
    string? Mascara,
    int LarguraColuna);
