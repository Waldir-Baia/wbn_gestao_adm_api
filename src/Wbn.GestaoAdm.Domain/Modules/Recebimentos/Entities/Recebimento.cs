using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class Recebimento : BaseEntity
{
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
        string? observacao)
    {
        EmpresaId = empresaId;
        UsuarioEnvioId = usuarioEnvioId;
        CodigoRecebimento = NormalizeRequired(codigoRecebimento);
        StatusRecebimento = Constants.StatusRecebimento.Pendente;
        Origem = NormalizeRequired(origem);
        Observacao = NormalizeOptional(observacao);
        DataEnvio = DateTime.UtcNow;
        DataRecebimento = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public ulong EmpresaId { get; private set; }
    public ulong UsuarioEnvioId { get; private set; }
    public string CodigoRecebimento { get; private set; } = string.Empty;
    public string StatusRecebimento { get; private set; } = Constants.StatusRecebimento.Pendente;
    public string Origem { get; private set; } = OrigensRecebimento.Mobile;
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

    public void AtualizarStatus(string statusRecebimento, string? observacao = null)
    {
        StatusRecebimento = NormalizeRequired(statusRecebimento);
        Observacao = NormalizeOptional(observacao);
        DataAtualizacao = DateTime.UtcNow;
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

        if (string.IsNullOrWhiteSpace(StatusRecebimento))
        {
            AddError("O status do recebimento e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Origem))
        {
            AddError("A origem do recebimento e obrigatoria.");
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
