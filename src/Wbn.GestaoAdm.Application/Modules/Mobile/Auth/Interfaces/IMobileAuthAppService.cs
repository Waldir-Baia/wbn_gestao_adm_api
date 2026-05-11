using Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Interfaces;

public interface IMobileAuthAppService
{
    Task<MobileLoginResult> AuthenticateAsync(MobileLoginRequest request, CancellationToken cancellationToken = default);
}
