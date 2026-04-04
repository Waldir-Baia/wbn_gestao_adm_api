using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class RecebimentoComentario : AuditableEntity
{
    private RecebimentoComentario()
    {
    }

    public RecebimentoComentario(
        ulong recebimentoId,
        ulong usuarioId,
        string comentario,
        bool visivelParaEmpresa)
    {
        RecebimentoId = recebimentoId;
        UsuarioId = usuarioId;
        Comentario = NormalizeRequired(comentario);
        VisivelParaEmpresa = visivelParaEmpresa;
        DefinirDataCadastro();
    }

    public ulong RecebimentoId { get; private set; }
    public ulong UsuarioId { get; private set; }
    public string Comentario { get; private set; } = string.Empty;
    public bool VisivelParaEmpresa { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Usuario Usuario { get; private set; } = null!;

    public void AtualizarComentario(string comentario, bool visivelParaEmpresa)
    {
        Comentario = NormalizeRequired(comentario);
        VisivelParaEmpresa = visivelParaEmpresa;
        DefinirDataAtualizacao();
    }

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento do comentario e obrigatorio.");
        }

        if (UsuarioId == 0)
        {
            AddError("O usuario do comentario e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Comentario))
        {
            AddError("O comentario e obrigatorio.");
        }

        return !HasErrors;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
