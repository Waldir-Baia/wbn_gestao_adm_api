namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileEnviarDocumentoRequest(
    ulong RecebimentoId,
    ulong TipoDocumentoId,
    ulong UsuarioId,
    string NomeOriginal,
    string NomeArquivo,
    string CaminhoArquivo,
    string? Extensao,
    string? MimeType,
    long TamanhoBytes,
    int OrdemExibicao);
