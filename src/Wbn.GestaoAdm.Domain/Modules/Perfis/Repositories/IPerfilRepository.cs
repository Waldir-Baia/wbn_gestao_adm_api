using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Perfis.Repositories;

public interface IPerfilRepository : IBaseRepository<Perfil>
{
    Task<Perfil?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default);
}
