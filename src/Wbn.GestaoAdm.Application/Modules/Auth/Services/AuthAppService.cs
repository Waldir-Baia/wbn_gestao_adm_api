using Wbn.GestaoAdm.Application.Common.Security;
using Wbn.GestaoAdm.Application.Modules.Auth.Dtos;
using Wbn.GestaoAdm.Application.Modules.Auth.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Auth.Services;

public sealed class AuthAppService(
    IUsuarioRepository usuarioRepository,
    IEmpresaRepository empresaRepository,
    IPasswordHasher passwordHasher) : IAuthAppService
{
    public async Task<AuthenticatedUserResult> AuthenticateAsync(
        AuthenticateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usuario = await usuarioRepository.GetByEmailForAuthenticationAsync(request.Email, cancellationToken)
            ?? throw new InvalidOperationException("E-mail ou senha inválidos.");

        if (!usuario.Ativo)
        {
            throw new InvalidOperationException("Usuário inativo.");
        }

        if (!passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            throw new InvalidOperationException("E-mail ou senha inválidos.");
        }

        var empresa = await empresaRepository.Get(request.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa informada não encontrada.");

        if (!empresa.Ativo)
        {
            throw new InvalidOperationException("Empresa informada está inativa.");
        }

        var empresaVinculada = usuario.UsuariosEmpresas
            .FirstOrDefault(vinculo => vinculo.EmpresaId == request.EmpresaId && vinculo.Ativo);

        if (empresaVinculada is null)
        {
            throw new InvalidOperationException("Usuário não possui acesso à empresa informada.");
        }

        usuario.RegistrarUltimoLogin();
        await usuarioRepository.Update(usuario, cancellationToken);

        return new AuthenticatedUserResult(
            usuario.Id,
            usuario.PerfilId,
            usuario.Nome,
            usuario.Email,
            usuario.Telefone,
            usuario.Ativo,
            usuario.UltimoLogin,
            new EmpresaSessaoResult(
                empresa.Id,
                empresa.NomeFantasia,
                empresa.RazaoSocial,
                empresa.Cnpj,
                empresa.Ativo));
    }
}
