using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Enums;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class RecebimentoDivergencia : AuditableEntity
{
    private static readonly IReadOnlyDictionary<DivergenciaStatusEnum, IReadOnlyCollection<DivergenciaStatusEnum>> AllowedStatusTransitions =
        new Dictionary<DivergenciaStatusEnum, IReadOnlyCollection<DivergenciaStatusEnum>>
        {
            [DivergenciaStatusEnum.Aberta] = [DivergenciaStatusEnum.EmAnalise, DivergenciaStatusEnum.Cancelada],
            [DivergenciaStatusEnum.EmAnalise] = [DivergenciaStatusEnum.Resolvida, DivergenciaStatusEnum.Cancelada],
            [DivergenciaStatusEnum.Resolvida] = [],
            [DivergenciaStatusEnum.Cancelada] = []
        };

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
        StatusDivergencia = DivergenciaStatusEnum.Aberta;
        Resolvida = false;
        DefinirDataCadastro();
    }

    public ulong RecebimentoId { get; private set; }
    public ulong UsuarioId { get; private set; }
    public string TipoDivergencia { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public DivergenciaStatusEnum StatusDivergencia { get; private set; } = DivergenciaStatusEnum.Aberta;
    public bool Resolvida { get; private set; }
    public DateTime? DataResolucao { get; private set; }
    public ulong? UsuarioResolucaoId { get; private set; }
    public string? ObservacaoResolucao { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Usuario Usuario { get; private set; } = null!;
    public Usuario? UsuarioResolucao { get; private set; }

    public void AlterarStatus(DivergenciaStatusEnum novoStatus)
    {
        if (!Enum.IsDefined(novoStatus))
        {
            throw new RegraDeNegocioException("O status da divergência informado é inválido.");
        }

        if (StatusDivergencia == novoStatus)
        {
            throw new RegraDeNegocioException("A divergência já se encontra no status informado.");
        }

        if (!AllowedStatusTransitions.TryGetValue(StatusDivergencia, out var allowedStatuses)
            || !allowedStatuses.Contains(novoStatus))
        {
            throw new RegraDeNegocioException(
                $"Nao e permitido alterar o status da divergência de {StatusDivergencia} para {novoStatus}.");
        }

        if (novoStatus == DivergenciaStatusEnum.Resolvida)
        {
            throw new RegraDeNegocioException("Use a operação de resolver para concluir a divergência.");
        }

        StatusDivergencia = novoStatus;
        DefinirDataAtualizacao();
    }

    public void Resolver(ulong usuarioResolucaoId, string observacaoResolucao)
    {
        if (StatusDivergencia != DivergenciaStatusEnum.EmAnalise)
        {
            throw new RegraDeNegocioException("Somente divergências em análise podem ser resolvidas.");
        }

        if (usuarioResolucaoId == 0)
        {
            throw new RegraDeNegocioException("O usuário de resolução da divergência é obrigatório.");
        }

        UsuarioResolucaoId = usuarioResolucaoId;
        ObservacaoResolucao = NormalizeRequired(observacaoResolucao);
        Resolvida = true;
        DataResolucao = DateTime.UtcNow;
        StatusDivergencia = DivergenciaStatusEnum.Resolvida;
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

        if (!Enum.IsDefined(StatusDivergencia))
        {
            AddError("O status da divergencia informado e invalido.");
        }

        if (StatusDivergencia == DivergenciaStatusEnum.Resolvida)
        {
            if (!Resolvida)
            {
                AddError("Uma divergencia resolvida deve estar marcada como resolvida.");
            }

            if (!DataResolucao.HasValue)
            {
                AddError("A data de resolucao da divergencia e obrigatoria quando resolvida.");
            }

            if (!UsuarioResolucaoId.HasValue)
            {
                AddError("O usuario de resolucao da divergencia e obrigatorio quando resolvida.");
            }

            if (string.IsNullOrWhiteSpace(ObservacaoResolucao))
            {
                AddError("A observacao de resolucao da divergencia e obrigatoria quando resolvida.");
            }
        }

        return !HasErrors;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }
}
