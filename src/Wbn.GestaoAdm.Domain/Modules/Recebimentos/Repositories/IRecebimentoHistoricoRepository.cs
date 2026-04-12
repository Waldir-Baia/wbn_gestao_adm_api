using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;

public interface IRecebimentoHistoricoRepository : IBaseRepository<RecebimentoHistorico>
{
    Task<IReadOnlyCollection<RecebimentoHistorico>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default);
}
