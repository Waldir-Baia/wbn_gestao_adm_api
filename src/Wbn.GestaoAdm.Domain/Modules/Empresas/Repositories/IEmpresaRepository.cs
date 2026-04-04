using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;

public interface IEmpresaRepository : IBaseRepository<Empresa>
{
    Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
}
