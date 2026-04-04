using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;

public interface ITipoDocumentoRepository : IBaseRepository<TipoDocumento>
{
    Task<TipoDocumento?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);
}
