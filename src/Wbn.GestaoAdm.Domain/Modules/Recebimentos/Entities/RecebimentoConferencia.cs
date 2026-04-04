using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class RecebimentoConferencia : BaseEntity
{
    private RecebimentoConferencia()
    {
    }

    public RecebimentoConferencia(
        ulong recebimentoId,
        ulong usuarioConferenciaId,
        string statusConferencia,
        bool notaEncontrada,
        bool boletoEncontrado,
        bool valorConfere,
        bool dataVencimentoConfere,
        bool documentoConfere,
        string? observacao,
        DateTime? dataConferencia = null)
    {
        RecebimentoId = recebimentoId;
        UsuarioConferenciaId = usuarioConferenciaId;
        StatusConferencia = NormalizeRequired(statusConferencia);
        NotaEncontrada = notaEncontrada;
        BoletoEncontrado = boletoEncontrado;
        ValorConfere = valorConfere;
        DataVencimentoConfere = dataVencimentoConfere;
        DocumentoConfere = documentoConfere;
        Observacao = NormalizeOptional(observacao);
        DataConferencia = dataConferencia ?? DateTime.UtcNow;
        DataCadastro = DateTime.UtcNow;
    }

    public ulong RecebimentoId { get; private set; }
    public ulong UsuarioConferenciaId { get; private set; }
    public string StatusConferencia { get; private set; } = string.Empty;
    public bool NotaEncontrada { get; private set; }
    public bool BoletoEncontrado { get; private set; }
    public bool ValorConfere { get; private set; }
    public bool DataVencimentoConfere { get; private set; }
    public bool DocumentoConfere { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime DataConferencia { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Usuario UsuarioConferencia { get; private set; } = null!;

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento da conferencia e obrigatorio.");
        }

        if (UsuarioConferenciaId == 0)
        {
            AddError("O usuario da conferencia e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(StatusConferencia))
        {
            AddError("O status da conferencia e obrigatorio.");
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
