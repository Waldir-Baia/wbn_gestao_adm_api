using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class Boleto : AuditableEntity
{
    private readonly List<NotaFiscalBoleto> _notasFiscaisBoletos = [];

    private Boleto()
    {
    }

    public Boleto(
        ulong recebimentoId,
        ulong? arquivoId,
        string? codigoBarras,
        string? linhaDigitavel,
        decimal valorBoleto,
        DateTime? dataVencimento,
        DateTime? dataEmissao,
        string? beneficiario,
        string? documentoBeneficiario,
        string? observacao)
    {
        RecebimentoId = recebimentoId;
        ArquivoId = arquivoId;
        CodigoBarras = NormalizeOptional(codigoBarras);
        LinhaDigitavel = NormalizeOptional(linhaDigitavel);
        ValorBoleto = valorBoleto;
        DataVencimento = dataVencimento;
        DataEmissao = dataEmissao;
        Beneficiario = NormalizeOptional(beneficiario);
        DocumentoBeneficiario = NormalizeDigitsOptional(documentoBeneficiario);
        Observacao = NormalizeOptional(observacao);
        DefinirDataCadastro();
    }

    public ulong RecebimentoId { get; private set; }
    public ulong? ArquivoId { get; private set; }
    public string? CodigoBarras { get; private set; }
    public string? LinhaDigitavel { get; private set; }
    public decimal ValorBoleto { get; private set; }
    public DateTime? DataVencimento { get; private set; }
    public DateTime? DataEmissao { get; private set; }
    public string? Beneficiario { get; private set; }
    public string? DocumentoBeneficiario { get; private set; }
    public string? Observacao { get; private set; }

    public Recebimento Recebimento { get; private set; } = null!;
    public Arquivo? Arquivo { get; private set; }
    public IReadOnlyCollection<NotaFiscalBoleto> NotasFiscaisBoletos => _notasFiscaisBoletos;

    public override bool Validate()
    {
        ClearErrors();

        if (RecebimentoId == 0)
        {
            AddError("O recebimento do boleto e obrigatorio.");
        }

        if (ValorBoleto < 0)
        {
            AddError("O valor do boleto nao pode ser negativo.");
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
