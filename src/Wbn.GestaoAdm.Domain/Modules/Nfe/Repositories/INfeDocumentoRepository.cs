using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;

public interface INfeDocumentoRepository : IBaseRepository<NfeDocumento>
{
    Task<NfeDocumento?> GetByChaveAcessoAsync(string chaveAcesso, CancellationToken cancellationToken = default);
    Task<NfeDocumento?> GetWithProdutosByChaveAcessoAsync(string chaveAcesso, CancellationToken cancellationToken = default);
    Task<List<NfeDocumento>> GetByEmpresaIdAsync(ulong empresaId, CancellationToken cancellationToken = default);
}
