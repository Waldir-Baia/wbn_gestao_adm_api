namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record IniciarConferenciaRequest(
    ulong UsuarioAcaoId,
    string? Observacao);
