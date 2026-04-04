using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Usuario?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
}
