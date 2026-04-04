using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class RecebimentoHistorico : BaseEntity
{
    private RecebimentoHistorico()
    {
    }

    public RecebimentoHistorico(ulong recebimentoId, ulong? usuarioId, string acao, string descricao)
    {
        RecebimentoId = recebimentoId;
        UsuarioId = usuarioId;
        Acao = NormalizeRequired(acao);
        Descricao = NormalizeRequired(descricao);
        DataCadastro = DateTime.UtcNow;
    }

    public ulong RecebimentoId { get; private set; }
    public ulong? UsuarioId { get; private set; }
    public string Acao { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public DateTime DataCadastro { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Usuario? Usuario { get; private set; }

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento do historico e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Acao))
        {
            AddError("A acao do historico e obrigatoria.");
        }

        if (string.IsNullOrWhiteSpace(Descricao))
        {
            AddError("A descricao do historico e obrigatoria.");
        }

        return !HasErrors;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
