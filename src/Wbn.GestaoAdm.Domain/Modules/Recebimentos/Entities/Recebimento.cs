using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Enums;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class Recebimento : BaseEntity
{
    private static readonly IReadOnlyDictionary<RecebimentoStatusEnum, IReadOnlyCollection<RecebimentoStatusEnum>> AllowedStatusTransitions =
        new Dictionary<RecebimentoStatusEnum, IReadOnlyCollection<RecebimentoStatusEnum>>
        {
            [RecebimentoStatusEnum.Pendente] = [RecebimentoStatusEnum.EmConferencia],
            [RecebimentoStatusEnum.EmConferencia] = [RecebimentoStatusEnum.Conferido, RecebimentoStatusEnum.ComDivergencia],
            [RecebimentoStatusEnum.ComDivergencia] = [RecebimentoStatusEnum.Finalizado],
            [RecebimentoStatusEnum.Conferido] = [],
            [RecebimentoStatusEnum.Finalizado] = []
        };

    private static readonly HashSet<string> AllowedOrigins =
    [
        Constants.OrigensRecebimento.Mobile,
        Constants.OrigensRecebimento.Web,
        Constants.OrigensRecebimento.Importacao
    ];

    private readonly List<Arquivo> _arquivos = [];
    private readonly List<NotaFiscal> _notasFiscais = [];
    private readonly List<Boleto> _boletos = [];
    private readonly List<RecebimentoConferencia> _conferencias = [];
    private readonly List<RecebimentoDivergencia> _divergencias = [];
    private readonly List<RecebimentoHistorico> _historicos = [];
    private readonly List<RecebimentoComentario> _comentarios = [];

    private Recebimento()
    {
    }

    public Recebimento(
        ulong empresaId,
        ulong usuarioEnvioId,
        string codigoRecebimento,
        string origem,
        string? observacao,
        DateTime? dataEnvio = null)
    {
        EmpresaId = empresaId;
        UsuarioEnvioId = usuarioEnvioId;
        CodigoRecebimento = NormalizeRequired(codigoRecebimento);
        StatusRecebimento = RecebimentoStatusEnum.Pendente;
        Origem = NormalizeRequired(origem).ToUpperInvariant();
        Observacao = NormalizeOptional(observacao);
        DataEnvio = dataEnvio ?? DateTime.UtcNow;
        DataRecebimento = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public ulong EmpresaId { get; private set; }
    public ulong UsuarioEnvioId { get; private set; }
    public string CodigoRecebimento { get; private set; } = string.Empty;
    public RecebimentoStatusEnum StatusRecebimento { get; private set; } = RecebimentoStatusEnum.Pendente;
    public string Origem { get; private set; } = Constants.OrigensRecebimento.Mobile;
    public string? Observacao { get; private set; }
    public DateTime DataEnvio { get; private set; }
    public DateTime DataRecebimento { get; private set; }
    public DateTime DataAtualizacao { get; private set; }

    public Empresa Empresa { get; private set; } = null!;
    public Usuario UsuarioEnvio { get; private set; } = null!;

    public IReadOnlyCollection<Arquivo> Arquivos => _arquivos;
    public IReadOnlyCollection<NotaFiscal> NotasFiscais => _notasFiscais;
    public IReadOnlyCollection<Boleto> Boletos => _boletos;
    public IReadOnlyCollection<RecebimentoConferencia> Conferencias => _conferencias;
    public IReadOnlyCollection<RecebimentoDivergencia> Divergencias => _divergencias;
    public IReadOnlyCollection<RecebimentoHistorico> Historicos => _historicos;
    public IReadOnlyCollection<RecebimentoComentario> Comentarios => _comentarios;

    public void IniciarConferencia(string? observacao = null)
    {
        if (StatusRecebimento != RecebimentoStatusEnum.Pendente)
        {
            throw new RegraDeNegocioException("Somente recebimentos pendentes podem iniciar conferência.");
        }

        AtualizarStatusInterno(RecebimentoStatusEnum.EmConferencia, observacao);
    }

    public void FinalizarConferencia(RecebimentoStatusEnum novoStatus, string? observacao = null)
    {
        if (StatusRecebimento != RecebimentoStatusEnum.EmConferencia)
        {
            throw new RegraDeNegocioException("Somente recebimentos em conferência podem ser finalizados.");
        }

        if (novoStatus is not RecebimentoStatusEnum.Conferido and not RecebimentoStatusEnum.ComDivergencia)
        {
            throw new RegraDeNegocioException("A finalização da conferência deve resultar em Conferido ou ComDivergencia.");
        }

        AtualizarStatusInterno(novoStatus, observacao);
    }

    public void AtualizarStatus(RecebimentoStatusEnum novoStatus, string? observacao = null)
    {
        AtualizarStatusInterno(novoStatus, observacao);
    }

    public override bool Validate()
    {
        ClearErrors();

        if (EmpresaId == 0)
        {
            AddError("A empresa do recebimento e obrigatoria.");
        }

        if (UsuarioEnvioId == 0)
        {
            AddError("O usuario de envio do recebimento e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(CodigoRecebimento))
        {
            AddError("O codigo do recebimento e obrigatorio.");
        }

        if (!Enum.IsDefined(StatusRecebimento))
        {
            AddError("O status do recebimento informado e invalido.");
        }

        if (string.IsNullOrWhiteSpace(Origem))
        {
            AddError("A origem do recebimento e obrigatoria.");
        }
        else if (!AllowedOrigins.Contains(Origem))
        {
            AddError("A origem do recebimento informada e invalida.");
        }

        return !HasErrors;
    }

    private void AtualizarStatusInterno(RecebimentoStatusEnum novoStatus, string? observacao)
    {
        if (!Enum.IsDefined(novoStatus))
        {
            throw new RegraDeNegocioException("O status informado para o recebimento e invalido.");
        }

        if (StatusRecebimento == novoStatus)
        {
            throw new RegraDeNegocioException("O recebimento ja se encontra no status informado.");
        }

        if (!AllowedStatusTransitions.TryGetValue(StatusRecebimento, out var nextStatuses)
            || !nextStatuses.Contains(novoStatus))
        {
            throw new RegraDeNegocioException(
                $"Nao e permitido alterar o status do recebimento de {StatusRecebimento} para {novoStatus}.");
        }

        StatusRecebimento = novoStatus;
        Observacao = NormalizeOptional(observacao);
        DataAtualizacao = DateTime.UtcNow;
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
