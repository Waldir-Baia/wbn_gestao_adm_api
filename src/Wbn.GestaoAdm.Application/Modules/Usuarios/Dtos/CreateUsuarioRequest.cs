namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;

public sealed record CreateUsuarioRequest(
    ulong PerfilId,
    string Nome,
    string Email,
    string Login,
    string SenhaHash,
    string? Telefone);
