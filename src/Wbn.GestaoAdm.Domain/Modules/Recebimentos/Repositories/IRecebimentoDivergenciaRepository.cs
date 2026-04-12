using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;

public interface IRecebimentoDivergenciaRepository : IBaseRepository<RecebimentoDivergencia>
{
    Task<RecebimentoDivergencia?> GetDetailsByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RecebimentoDivergencia>> GetByRecebimentoIdAsync(ulong recebimentoId, CancellationToken cancellationToken = default);
}
