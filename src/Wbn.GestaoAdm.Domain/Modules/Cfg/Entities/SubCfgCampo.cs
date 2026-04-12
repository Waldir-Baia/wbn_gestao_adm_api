using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;

public sealed class SubCfgCampo : BaseEntity
{
    private SubCfgCampo()
    {
    }

    public SubCfgCampo(
        string identificador,
        string identificadorCfg,
        string campo,
        string descricao,
        string tipoDados,
        int ordemCampo,
        bool permitirFiltro,
        bool visivel,
        string campoBusca,
        string? mascara = null,
        int larguraColuna = 100)
    {
        Identificador = NormalizeRequired(identificador);
        IdentificadorCfg = NormalizeRequired(identificadorCfg);
        Campo = NormalizeRequired(campo);
        Descricao = NormalizeRequired(descricao);
        TipoDados = NormalizeRequired(tipoDados);
        OrdemCampo = ordemCampo;
        PermitirFiltro = permitirFiltro;
        Visivel = visivel;
        CampoBusca = NormalizeRequired(campoBusca);
        Mascara = NormalizeOptional(mascara);
        LarguraColuna = larguraColuna;
    }

    public string Identificador { get; private set; } = string.Empty;
    public string IdentificadorCfg { get; private set; } = string.Empty;
    public string Campo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public string TipoDados { get; private set; } = string.Empty;
    public int OrdemCampo { get; private set; }
    public bool PermitirFiltro { get; private set; }
    public bool Visivel { get; private set; }
    public string CampoBusca { get; private set; } = string.Empty;
    public string? Mascara { get; private set; }
    public int LarguraColuna { get; private set; }

    public CfgConsulta CfgConsulta { get; private set; } = null!;

    public override bool Validate()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(IdentificadorCfg))
        {
            AddError("O identificador do CFG e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Campo))
        {
            AddError("O campo do SubCFG e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(CampoBusca))
        {
            AddError("O campo de busca do SubCFG e obrigatorio.");
        }

        return !HasErrors;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
