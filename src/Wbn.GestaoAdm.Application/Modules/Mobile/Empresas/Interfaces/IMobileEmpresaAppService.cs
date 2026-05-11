using Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Interfaces;

public interface IMobileEmpresaAppService
{
    Task<IReadOnlyCollection<MobileEmpresaResponse>> GetEmpresasDoUsuarioAsync(
        ulong usuarioId,
        CancellationToken cancellationToken = default);
}
