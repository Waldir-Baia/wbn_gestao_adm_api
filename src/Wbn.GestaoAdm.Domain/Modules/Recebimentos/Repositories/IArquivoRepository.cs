using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;

public interface IArquivoRepository : IBaseRepository<Arquivo>
{
    Task<Arquivo?> GetDetailsByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Arquivo>> GetByRecebimentoIdAsync(ulong recebimentoId, CancellationToken cancellationToken = default);
}
