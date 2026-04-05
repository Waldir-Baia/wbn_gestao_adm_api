using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Empresas.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Empresas.Interfaces;

public interface IEmpresaAppService : IAppService
{
    Task<IReadOnlyCollection<EmpresaLookupResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
