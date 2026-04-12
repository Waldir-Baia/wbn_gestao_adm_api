namespace Wbn.GestaoAdm.Application.Modules.Auth.Dtos;

public sealed record AuthenticatedUserResult(
    ulong Id,
    ulong PerfilId,
    string Nome,
    string Email,
    string? Telefone,
    bool Ativo,
    DateTime? UltimoLogin,
    EmpresaSessaoResult Empresa);

public sealed record EmpresaSessaoResult(
    ulong Id,
    string NomeFantasia,
    string RazaoSocial,
    string Cnpj,
    bool Ativo);
