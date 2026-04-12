namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;

public sealed record UsuarioResponse(
    ulong Id,
    ulong PerfilId,
    ulong EmpresaId,
    string Nome,
    string Email,
    string Login,
    string? Telefone,
    bool Ativo,
    DateTime? UltimoLogin,
    DateTime DataCadastro,
    DateTime? DataAtualizacao);
