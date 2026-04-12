using Wbn.GestaoAdm.Application.Modules.Auth.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Auth.Interfaces;

public interface IAuthAppService
{
    Task<AuthenticatedUserResult> AuthenticateAsync(AuthenticateUserRequest request, CancellationToken cancellationToken = default);
}
