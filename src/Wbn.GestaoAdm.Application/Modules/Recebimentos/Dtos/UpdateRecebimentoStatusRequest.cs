namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

public sealed record UpdateRecebimentoStatusRequest(
    int Status,
    ulong? UsuarioAcaoId,
    string? Observacao);
