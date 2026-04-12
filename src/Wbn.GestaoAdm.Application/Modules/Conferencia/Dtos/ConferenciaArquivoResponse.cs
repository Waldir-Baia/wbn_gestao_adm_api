namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record ConferenciaArquivoResponse(
    ulong Id,
    ulong TipoDocumentoId,
    string NomeOriginal,
    string NomeArquivo,
    string CaminhoArquivo,
    string? Extensao,
    string? MimeType,
    long TamanhoBytes,
    int OrdemExibicao,
    bool Ativo,
    DateTime DataUpload);
