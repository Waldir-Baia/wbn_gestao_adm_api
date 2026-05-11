using Wbn.GestaoAdm.Application.Common.Security;
using Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Services;

public sealed class MobileAuthAppService(
    IUsuarioRepository usuarioRepository,
    IEmpresaRepository empresaRepository,
    IPasswordHasher passwordHasher) : IMobileAuthAppService
{
    public async Task<MobileLoginResult> AuthenticateAsync(
        MobileLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usuario = await usuarioRepository.GetByEmailForAuthenticationAsync(request.Email, cancellationToken)
            ?? throw new InvalidOperationException("E-mail ou senha invalidos.");

        if (!usuario.Ativo)
        {
            throw new InvalidOperationException("Usuario inativo.");
        }

        if (!passwordHasher.Verify(request.Senha, usuario.SenhaHash))
        {
            throw new InvalidOperationException("E-mail ou senha invalidos.");
        }

        var empresaIds = usuario.UsuariosEmpresas
            .Where(v => v.Ativo)
            .Select(v => v.EmpresaId)
            .ToList();

        var empresas = await empresaRepository.GetAll(
            e => empresaIds.Contains(e.Id) && e.Ativo,
            cancellationToken);

        usuario.RegistrarUltimoLogin();
        await usuarioRepository.Update(usuario, cancellationToken);

        var empresasVinculadas = empresas
            .OrderBy(e => e.NomeFantasia)
            .Select(e => new MobileEmpresaVinculadaResult(e.Id, e.NomeFantasia))
            .ToArray();

        return new MobileLoginResult(
            usuario.Id,
            usuario.PerfilId,
            usuario.Nome,
            usuario.Email,
            usuario.Telefone,
            empresasVinculadas);
    }
}
