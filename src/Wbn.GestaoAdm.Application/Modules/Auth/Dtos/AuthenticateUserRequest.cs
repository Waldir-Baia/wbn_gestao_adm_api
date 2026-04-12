namespace Wbn.GestaoAdm.Application.Modules.Auth.Dtos;

public sealed record AuthenticateUserRequest(
    ulong EmpresaId,
    string Email,
    string Senha);
