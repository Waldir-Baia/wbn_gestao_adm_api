using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class RecebimentoDivergencia : AuditableEntity
{
    private RecebimentoDivergencia()
    {
    }

    public RecebimentoDivergencia(
        ulong recebimentoId,
        ulong usuarioId,
        string tipoDivergencia,
        string descricao)
    {
        RecebimentoId = recebimentoId;
        UsuarioId = usuarioId;
        TipoDivergencia = NormalizeRequired(tipoDivergencia);
        Descricao = NormalizeRequired(descricao);
        Resolvida = false;
        DefinirDataCadastro();
    }

    public ulong RecebimentoId { get; private set; }
    public ulong UsuarioId { get; private set; }
    public string TipoDivergencia { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public bool Resolvida { get; private set; }
    public DateTime? DataResolucao { get; private set; }
    public ulong? UsuarioResolucaoId { get; private set; }
    public string? ObservacaoResolucao { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Usuario Usuario { get; private set; } = null!;
    public Usuario? UsuarioResolucao { get; private set; }

    public void Resolver(ulong usuarioResolucaoId, string? observacaoResolucao)
    {
        UsuarioResolucaoId = usuarioResolucaoId;
        ObservacaoResolucao = NormalizeOptional(observacaoResolucao);
        Resolvida = true;
        DataResolucao = DateTime.UtcNow;
        DefinirDataAtualizacao();
    }

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento da divergencia e obrigatorio.");
        }

        if (UsuarioId == 0)
        {
            AddError("O usuario da divergencia e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(TipoDivergencia))
        {
            AddError("O tipo da divergencia e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Descricao))
        {
            AddError("A descricao da divergencia e obrigatoria.");
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
