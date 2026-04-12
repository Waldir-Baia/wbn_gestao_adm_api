using Wbn.GestaoAdm.Application.Abstractions.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Documentos.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Documentos.Interfaces;

public interface IDocumentoAppService : IAppService
{
    Task<CriarDocumentoResponse> CreateAsync(CriarDocumentoRequest request, CancellationToken cancellationToken = default);
    Task<DocumentoResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DocumentoResponse>> GetByRecebimentoIdAsync(ulong recebimentoId, CancellationToken cancellationToken = default);
    Task<DocumentoResponse?> InativarAsync(ulong id, InativarDocumentoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TipoDocumentoResponse>> GetTiposDocumentoAtivosAsync(CancellationToken cancellationToken = default);
}
