namespace Wbn.GestaoAdm.Api.Contracts.Auth;

public sealed record LoginRequest(
    ulong EmpresaId,
    string Email,
    string Senha);
