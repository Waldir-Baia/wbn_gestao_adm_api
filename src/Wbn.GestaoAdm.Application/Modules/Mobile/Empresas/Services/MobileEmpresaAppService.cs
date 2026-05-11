using Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Services;

public sealed class MobileEmpresaAppService(
    IUsuarioRepository usuarioRepository,
    IEmpresaRepository empresaRepository) : IMobileEmpresaAppService
{
    public async Task<IReadOnlyCollection<MobileEmpresaResponse>> GetEmpresasDoUsuarioAsync(
        ulong usuarioId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await usuarioRepository.GetByIdForAuthenticationAsync(usuarioId, cancellationToken)
            ?? throw new RegraDeNegocioException("Usuario nao encontrado.");

        var empresaIds = usuario.UsuariosEmpresas
            .Where(v => v.Ativo)
            .Select(v => v.EmpresaId)
            .ToList();

        if (empresaIds.Count == 0)
        {
            return [];
        }

        var empresas = await empresaRepository.GetAll(
            e => empresaIds.Contains(e.Id) && e.Ativo,
            cancellationToken);

        return empresas
            .OrderBy(e => e.NomeFantasia)
            .Select(e => new MobileEmpresaResponse(e.Id, e.NomeFantasia))
            .ToArray();
    }
}
