using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;

public sealed class TipoDocumento : BaseEntity
{
    private readonly List<Arquivo> _arquivos = [];

    private TipoDocumento()
    {
    }

    public TipoDocumento(string nome, string? descricao, bool ativo = true)
    {
        Nome = NormalizeRequired(nome);
        Descricao = NormalizeOptional(descricao);
        Ativo = ativo;
        DataCadastro = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public IReadOnlyCollection<Arquivo> Arquivos => _arquivos;

    public void Atualizar(string nome, string? descricao, bool ativo)
    {
        Nome = NormalizeRequired(nome);
        Descricao = NormalizeOptional(descricao);
        Ativo = ativo;
    }

    public void Inativar()
    {
        Ativo = false;
    }

    public override bool Validate()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(Nome))
        {
            AddError("O nome do tipo de documento e obrigatorio.");
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
