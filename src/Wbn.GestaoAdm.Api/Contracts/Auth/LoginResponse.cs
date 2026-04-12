namespace Wbn.GestaoAdm.Api.Contracts.Auth;

public sealed record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    AuthenticatedUserResponse Usuario,
    EmpresaSessaoResponse Empresa);

public sealed record AuthenticatedUserResponse(
    ulong Id,
    ulong PerfilId,
    string Nome,
    string Email,
    string? Telefone,
    bool Ativo,
    DateTime? UltimoLogin);

public sealed record EmpresaSessaoResponse(
    ulong Id,
    string NomeFantasia,
    string RazaoSocial,
    string Cnpj,
    bool Ativo);
