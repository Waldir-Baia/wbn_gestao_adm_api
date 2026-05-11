namespace Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;

public sealed record NfeProdutoResponse(
    ulong Id,
    ulong NfeDocumentoId,
    string? CodigoProduto,
    string NomeProduto,
    string? Descricao,
    string? Ncm,
    string? Cfop,
    string? Unidade,
    decimal? Quantidade,
    decimal? ValorUnitario,
    decimal? ValorTotal,
    string? Ean);
