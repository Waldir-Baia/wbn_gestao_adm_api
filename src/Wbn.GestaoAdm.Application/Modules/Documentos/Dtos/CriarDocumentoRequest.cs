namespace Wbn.GestaoAdm.Application.Modules.Documentos.Dtos;

public sealed record CriarDocumentoRequest(
    ulong RecebimentoId,
    ulong TipoDocumentoId,
    string NomeOriginal,
    string NomeArquivo,
    string CaminhoArquivo,
    string? Extensao,
    string? MimeType,
    long TamanhoBytes,
    int OrdemExibicao,
    ulong? UsuarioAcaoId);
