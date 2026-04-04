using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class NotaFiscal : AuditableEntity
{
    private readonly List<NotaFiscalBoleto> _notasFiscaisBoletos = [];

    private NotaFiscal()
    {
    }

    public NotaFiscal(
        ulong recebimentoId,
        ulong? arquivoId,
        string? numeroNota,
        string? serie,
        string? chaveAcesso,
        decimal valorTotal,
        DateTime? dataEmissao,
        DateTime? dataEntrada,
        string? cpfCnpjEmitente,
        string? nomeEmitente,
        string? observacao)
    {
        RecebimentoId = recebimentoId;
        ArquivoId = arquivoId;
        NumeroNota = NormalizeOptional(numeroNota);
        Serie = NormalizeOptional(serie);
        ChaveAcesso = NormalizeOptional(chaveAcesso);
        ValorTotal = valorTotal;
        DataEmissao = dataEmissao;
        DataEntrada = dataEntrada;
        CpfCnpjEmitente = NormalizeDigitsOptional(cpfCnpjEmitente);
        NomeEmitente = NormalizeOptional(nomeEmitente);
        Observacao = NormalizeOptional(observacao);
        DefinirDataCadastro();
    }

    public ulong RecebimentoId { get; private set; }
    public ulong? ArquivoId { get; private set; }
    public string? NumeroNota { get; private set; }
    public string? Serie { get; private set; }
    public string? ChaveAcesso { get; private set; }
    public decimal ValorTotal { get; private set; }
    public DateTime? DataEmissao { get; private set; }
    public DateTime? DataEntrada { get; private set; }
    public string? CpfCnpjEmitente { get; private set; }
    public string? NomeEmitente { get; private set; }
    public string? Observacao { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Arquivo? Arquivo { get; private set; }
    public IReadOnlyCollection<NotaFiscalBoleto> NotasFiscaisBoletos => _notasFiscaisBoletos;

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento da nota fiscal e obrigatorio.");
        }

        if (ValorTotal < 0)
        {
            AddError("O valor total da nota fiscal nao pode ser negativo.");
        }

        return !HasErrors;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeDigitsOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : new string(value.Where(char.IsDigit).ToArray());
    }
}
