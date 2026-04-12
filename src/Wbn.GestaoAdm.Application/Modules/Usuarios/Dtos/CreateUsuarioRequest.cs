namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;

public sealed record CreateUsuarioRequest(
    ulong PerfilId,
    ulong EmpresaId,
    string Nome,
    string Email,
    string Senha,
    string? Telefone);
