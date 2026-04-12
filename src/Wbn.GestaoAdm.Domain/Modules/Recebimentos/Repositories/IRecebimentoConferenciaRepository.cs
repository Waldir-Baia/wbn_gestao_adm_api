using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;

public interface IRecebimentoConferenciaRepository : IBaseRepository<RecebimentoConferencia>
{
    Task<IReadOnlyCollection<RecebimentoConferencia>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default);
}
