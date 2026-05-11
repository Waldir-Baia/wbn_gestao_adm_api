using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;

public interface INfeProdutoRepository : IBaseRepository<NfeProduto>
{
    Task<List<NfeProduto>> GetByNfeDocumentoIdAsync(ulong nfeDocumentoId, CancellationToken cancellationToken = default);
}
