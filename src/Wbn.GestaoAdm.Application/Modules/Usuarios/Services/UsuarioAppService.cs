using Wbn.GestaoAdm.Application.Common.Security;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;
using Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;

namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Services;

public sealed class UsuarioAppService(
    IUsuarioRepository usuarioRepository,
    IPerfilRepository perfilRepository,
    IEmpresaRepository empresaRepository,
    IPasswordHasher passwordHasher) : IUsuarioAppService
{
    public async Task<UsuarioResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.GetByIdForAuthenticationAsync(id, cancellationToken);
        return usuario is null ? null : MapToResponse(usuario);
    }

    public async Task<IReadOnlyCollection<UsuarioResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await usuarioRepository.GetAll(cancellationToken);
        var result = new List<UsuarioResponse>(usuarios.Count);

        foreach (var usuario in usuarios)
        {
            var usuarioCompleto = await usuarioRepository.GetByIdForAuthenticationAsync(usuario.Id, cancellationToken);
            if (usuarioCompleto is not null)
            {
                result.Add(MapToResponse(usuarioCompleto));
            }
        }

        return result;
    }

    public async Task<UsuarioResponse> CreateAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        await EnsurePerfilExistsAsync(request.PerfilId, cancellationToken);
        await EnsureEmpresaExistsAsync(request.EmpresaId, cancellationToken);
        await EnsureUniqueFieldsAsync(request.Email, null, cancellationToken);

        var usuario = new Usuario(
            request.PerfilId,
            request.Nome,
            request.Email,
            request.Email,
            passwordHasher.Hash(request.Senha),
            request.Telefone);

        if (!usuario.Validate())
        {
            throw new InvalidOperationException(string.Join(" ", usuario.Errors));
        }

        await usuarioRepository.Create(usuario, cancellationToken);

        usuario.UsuariosEmpresasInternal.Add(new UsuarioEmpresa(usuario.Id, request.EmpresaId));
        await usuarioRepository.Update(usuario, cancellationToken);

        return MapToResponse(usuario);
    }

    public async Task<UsuarioResponse?> UpdateAsync(ulong id, UpdateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.GetByIdForAuthenticationAsync(id, cancellationToken);

        if (usuario is null)
        {
            return null;
        }

        await EnsurePerfilExistsAsync(request.PerfilId, cancellationToken);
        await EnsureEmpresaExistsAsync(request.EmpresaId, cancellationToken);
        await EnsureUniqueFieldsAsync(request.Email, usuario.Id, cancellationToken);

        var senhaHash = string.IsNullOrWhiteSpace(request.Senha)
            ? usuario.SenhaHash
            : passwordHasher.Hash(request.Senha);

        usuario.Atualizar(
            request.PerfilId,
            request.Nome,
            request.Email,
            request.Email,
            senhaHash,
            request.Telefone,
            request.Ativo);

        if (!usuario.Validate())
        {
            throw new InvalidOperationException(string.Join(" ", usuario.Errors));
        }

        SincronizarEmpresas(usuario, request.EmpresaId);
        await usuarioRepository.Update(usuario, cancellationToken);

        return MapToResponse(usuario);
    }

    public async Task<bool> DeleteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.Get(id, cancellationToken);

        if (usuario is null)
        {
            return false;
        }

        await usuarioRepository.Remove(id, cancellationToken);
        return true;
    }

    private async Task EnsurePerfilExistsAsync(ulong perfilId, CancellationToken cancellationToken)
    {
        if (!await perfilRepository.RecordExists(perfilId, cancellationToken))
        {
            throw new InvalidOperationException("O perfil informado nao existe.");
        }
    }

    private async Task EnsureEmpresaExistsAsync(ulong empresaId, CancellationToken cancellationToken)
    {
        if (!await empresaRepository.RecordExists(empresaId, cancellationToken))
        {
            throw new InvalidOperationException("A empresa informada nao existe.");
        }
    }

    private async Task EnsureUniqueFieldsAsync(
        string email,
        ulong? usuarioId = null,
        CancellationToken cancellationToken = default)
    {
        var usuarioComMesmoEmail = await usuarioRepository.GetByEmailAsync(email, cancellationToken);

        if (usuarioComMesmoEmail is not null && usuarioComMesmoEmail.Id != usuarioId)
        {
            throw new InvalidOperationException("Ja existe um usuario cadastrado com este e-mail.");
        }
    }

    private static void SincronizarEmpresas(Usuario usuario, ulong empresaId)
    {
        var vinculoAtivo = usuario.UsuariosEmpresasInternal.FirstOrDefault(vinculo => vinculo.EmpresaId == empresaId);

        foreach (var vinculo in usuario.UsuariosEmpresasInternal)
        {
            vinculo.AtualizarStatus(vinculo.EmpresaId == empresaId);
        }

        if (vinculoAtivo is null)
        {
            usuario.UsuariosEmpresasInternal.Add(new UsuarioEmpresa(usuario.Id, empresaId));
        }
    }

    private static UsuarioResponse MapToResponse(Usuario usuario)
    {
        var empresaId = usuario.UsuariosEmpresas.FirstOrDefault(vinculo => vinculo.Ativo)?.EmpresaId ?? 0;

        return new UsuarioResponse(
            usuario.Id,
            usuario.PerfilId,
            empresaId,
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
