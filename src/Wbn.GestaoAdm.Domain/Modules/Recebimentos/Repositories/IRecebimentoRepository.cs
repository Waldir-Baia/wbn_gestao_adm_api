using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;

public interface IRecebimentoRepository : IBaseRepository<Recebimento>
{
    Task<Recebimento?> GetByCodigoRecebimentoAsync(string codigoRecebimento, CancellationToken cancellationToken = default);
    Task<Recebimento?> GetDetailsByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<List<Recebimento>> GetByUsuarioEnvioIdAsync(ulong usuarioId, CancellationToken cancellationToken = default);
}
