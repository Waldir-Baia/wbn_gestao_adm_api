using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class RecebimentoConferencia : BaseEntity
{
    private static readonly HashSet<string> AllowedStatuses =
    [
        Constants.StatusConferencia.Pendente,
        Constants.StatusConferencia.Aprovada,
        Constants.StatusConferencia.Reprovada
    ];

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
        bool documentoLegivel,
        string? observacao,
        DateTime? dataConferencia = null)
    {
        RecebimentoId = recebimentoId;
        UsuarioConferenciaId = usuarioConferenciaId;
        StatusConferencia = NormalizeStatus(statusConferencia);
        NotaEncontrada = notaEncontrada;
        BoletoEncontrado = boletoEncontrado;
        ValorConfere = valorConfere;
        DataVencimentoConfere = dataVencimentoConfere;
        DocumentoConfere = documentoLegivel;
        Observacao = NormalizeOptional(observacao);
        DataConferencia = dataConferencia ?? DateTime.UtcNow;
        DataCadastro = DateTime.UtcNow;

        ValidarConsistenciaResultado();
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

    public bool PossuiDivergencia()
    {
        return !NotaEncontrada
            || !BoletoEncontrado
            || !ValorConfere
            || !DataVencimentoConfere
            || !DocumentoConfere
            || string.Equals(StatusConferencia, Constants.StatusConferencia.Reprovada, StringComparison.OrdinalIgnoreCase);
    }

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
        else if (!AllowedStatuses.Contains(StatusConferencia))
        {
            AddError("O status da conferencia informado e invalido.");
        }

        return !HasErrors;
    }

    private void ValidarConsistenciaResultado()
    {
        var possuiDivergencia = PossuiDivergencia();

        if (string.Equals(StatusConferencia, Constants.StatusConferencia.Aprovada, StringComparison.OrdinalIgnoreCase) && possuiDivergencia)
        {
            throw new RegraDeNegocioException("Uma conferência aprovada exige todos os indicadores como verdadeiros.");
        }

        if (string.Equals(StatusConferencia, Constants.StatusConferencia.Reprovada, StringComparison.OrdinalIgnoreCase) && !possuiDivergencia)
        {
            throw new RegraDeNegocioException("Uma conferência reprovada exige ao menos uma divergência informada.");
        }

        if (string.Equals(StatusConferencia, Constants.StatusConferencia.Pendente, StringComparison.OrdinalIgnoreCase))
        {
            throw new RegraDeNegocioException("Nao e permitido finalizar a conferência com status pendente.");
        }
    }

    private static string NormalizeStatus(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
