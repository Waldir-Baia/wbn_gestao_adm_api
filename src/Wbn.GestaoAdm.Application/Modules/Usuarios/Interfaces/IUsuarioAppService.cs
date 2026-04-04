using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Usuarios.Interfaces;

public interface IUsuarioAppService : ICrudAppService<UsuarioResponse, ulong, CreateUsuarioRequest, UpdateUsuarioRequest>
{
}
