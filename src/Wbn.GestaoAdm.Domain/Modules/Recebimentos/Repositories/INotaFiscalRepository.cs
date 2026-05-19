using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;

public interface INotaFiscalRepository : IBaseRepository<NotaFiscal>
{
    Task<NotaFiscal?> GetByChaveAcessoAsync(
        string chaveAcesso,
        CancellationToken cancellationToken = default);
}
