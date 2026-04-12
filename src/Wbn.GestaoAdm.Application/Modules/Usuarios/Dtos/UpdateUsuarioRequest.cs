namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;

public sealed record UpdateUsuarioRequest(
    ulong PerfilId,
    ulong EmpresaId,
    string Nome,
    string Email,
    string? Senha,
    string? Telefone,
    bool Ativo);
