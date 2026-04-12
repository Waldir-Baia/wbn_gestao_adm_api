namespace Wbn.GestaoAdm.Application.Modules.Documentos.Dtos;

public sealed record DocumentoResponse(
    ulong Id,
    ulong RecebimentoId,
    ulong TipoDocumentoId,
    string TipoDocumentoNome,
    string NomeOriginal,
    string NomeArquivo,
    string CaminhoArquivo,
    string? Extensao,
    string? MimeType,
    long TamanhoBytes,
    int OrdemExibicao,
    bool Ativo,
    DateTime DataUpload);
