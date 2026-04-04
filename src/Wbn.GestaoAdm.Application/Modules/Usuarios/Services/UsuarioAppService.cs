using Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Services;

public sealed class UsuarioAppService(
    IUsuarioRepository usuarioRepository,
    IPerfilRepository perfilRepository) : IUsuarioAppService
{
    public async Task<UsuarioResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.Get(id);
        return usuario is null ? null : MapToResponse(usuario);
    }

    public async Task<IReadOnlyCollection<UsuarioResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await usuarioRepository.GetAll();
        return usuarios.Select(MapToResponse).ToArray();
    }

    public async Task<UsuarioResponse> CreateAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        await EnsurePerfilExistsAsync(request.PerfilId);
        await EnsureUniqueFieldsAsync(request.Email, request.Login);

        var usuario = new Usuario(
            request.PerfilId,
            request.Nome,
            request.Email,
            request.Login,
            request.SenhaHash,
            request.Telefone);

        if (!usuario.Validate())
        {
            throw new InvalidOperationException(string.Join(" ", usuario.Errors));
        }

        await usuarioRepository.Create(usuario);

        return MapToResponse(usuario);
    }

    public async Task<UsuarioResponse?> UpdateAsync(ulong id, UpdateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.Get(id);

        if (usuario is null)
        {
            return null;
        }

        await EnsurePerfilExistsAsync(request.PerfilId);
        await EnsureUniqueFieldsAsync(request.Email, request.Login, usuario.Id);

        usuario.Atualizar(
            request.PerfilId,
            request.Nome,
            request.Email,
            request.Login,
            request.SenhaHash,
            request.Telefone,
            request.Ativo);

        if (!usuario.Validate())
        {
            throw new InvalidOperationException(string.Join(" ", usuario.Errors));
        }

        await usuarioRepository.Update(usuario);

        return MapToResponse(usuario);
    }

    public async Task<bool> DeleteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.Get(id);

        if (usuario is null)
        {
            return false;
        }

        await usuarioRepository.Remove(id);
        return true;
    }

    private async Task EnsurePerfilExistsAsync(ulong perfilId)
    {
        if (!await perfilRepository.RecordExists(perfilId))
        {
            throw new InvalidOperationException("O perfil informado nao existe.");
        }
    }

    private async Task EnsureUniqueFieldsAsync(string email, string login, ulong? usuarioId = null)
    {
        var usuarioComMesmoEmail = await usuarioRepository.GetByEmailAsync(email);

        if (usuarioComMesmoEmail is not null && usuarioComMesmoEmail.Id != usuarioId)
        {
            throw new InvalidOperationException("Ja existe um usuario cadastrado com este e-mail.");
        }

        var usuarioComMesmoLogin = await usuarioRepository.GetByLoginAsync(login);

        if (usuarioComMesmoLogin is not null && usuarioComMesmoLogin.Id != usuarioId)
        {
            throw new InvalidOperationException("Ja existe um usuario cadastrado com este login.");
        }
    }

    private static UsuarioResponse MapToResponse(Usuario usuario)
    {
        return new UsuarioResponse(
            usuario.Id,
            usuario.PerfilId,
            usuario.Nome,
            usuario.Email,
            usuario.Login,
            usuario.Telefone,
            usuario.Ativo,
            usuario.UltimoLogin,
            usuario.DataCadastro,
            usuario.DataAtualizacao);
    }
}
