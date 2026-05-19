namespace Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;

public sealed record BuscarNfeResponse(
    ulong Id,
    ulong EmpresaId,
    string ChaveAcesso,
    long? Nsu,
    string TipoDocumento,
    string? CnpjEmitente,
    string? NomeEmitente,
    string? CnpjDestinatario,
    string? NomeDestinatario,
    string? NumeroNota,
    string? Serie,
    DateTime? DataEmissao,
    decimal? ValorTotal,
    string StatusManifestacao,
    DateTime? DataDownload,
    DateTime DataCriacao,
    DateTime? DataAtualizacao,
    IReadOnlyList<NfeProdutoResponse> Produtos);
