using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;

public sealed class CfgConsulta : BaseEntity
{
    private readonly List<SubCfgCampo> _subCfgCampos = [];

    private CfgConsulta()
    {
    }

    public CfgConsulta(
        string identificador,
        string descricao,
        string campoChavePrimaria,
        string sql,
        string? buscaIdentificador = null,
        string? buscaDescricao = null,
        string? buscarCodigoAlternativo = null,
        string? observacao = null)
    {
        Identificador = NormalizeRequired(identificador);
        Descricao = NormalizeRequired(descricao);
        CampoChavePrimaria = NormalizeRequired(campoChavePrimaria);
        Sql = NormalizeRequired(sql);
        BuscaIdentificador = NormalizeOptional(buscaIdentificador);
        BuscaDescricao = NormalizeOptional(buscaDescricao);
        BuscarCodigoAlternativo = NormalizeOptional(buscarCodigoAlternativo);
        Observacao = NormalizeOptional(observacao);
    }

    public string Identificador { get; private set; } = string.Empty;
    public string? BuscaIdentificador { get; private set; }
    public string? BuscaDescricao { get; private set; }
    public string? BuscarCodigoAlternativo { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public string CampoChavePrimaria { get; private set; } = string.Empty;
    public string Sql { get; private set; } = string.Empty;
    public string? Observacao { get; private set; }

    public IReadOnlyCollection<SubCfgCampo> SubCfgCampos => _subCfgCampos;

    public override bool Validate()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(Identificador))
        {
            AddError("O identificador do CFG e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Sql))
        {
            AddError("O SQL do CFG e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(CampoChavePrimaria))
        {
            AddError("O campo chave primaria do CFG e obrigatorio.");
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
