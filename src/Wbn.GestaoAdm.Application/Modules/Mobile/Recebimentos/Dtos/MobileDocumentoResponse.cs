namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileDocumentoResponse(
    ulong Id,
    ulong TipoDocumentoId,
    string TipoDocumentoNome,
    string NomeOriginal,
    string? Extensao,
    string? MimeType,
    long TamanhoBytes,
    int OrdemExibicao,
    DateTime DataUpload);
