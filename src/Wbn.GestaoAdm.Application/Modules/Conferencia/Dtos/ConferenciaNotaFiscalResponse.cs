namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record ConferenciaNotaFiscalResponse(
    ulong Id,
    ulong? ArquivoId,
    string? NumeroNota,
    string? Serie,
    string? ChaveAcesso,
    decimal ValorTotal,
    DateTime? DataEmissao,
    DateTime? DataEntrada,
    string? CpfCnpjEmitente,
    string? NomeEmitente,
    string? Observacao);
