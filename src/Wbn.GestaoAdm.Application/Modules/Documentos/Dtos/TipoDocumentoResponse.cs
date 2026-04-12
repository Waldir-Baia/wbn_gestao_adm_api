namespace Wbn.GestaoAdm.Application.Modules.Documentos.Dtos;

public sealed record TipoDocumentoResponse(
    ulong Id,
    string Nome,
    string? Descricao,
    bool Ativo,
    DateTime DataCadastro);
