namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record ConferenciaBoletoResponse(
    ulong Id,
    ulong? ArquivoId,
    string? CodigoBarras,
    string? LinhaDigitavel,
    decimal ValorBoleto,
    DateTime? DataVencimento,
    DateTime? DataEmissao,
    string? Beneficiario,
    string? DocumentoBeneficiario,
    string? Observacao);
