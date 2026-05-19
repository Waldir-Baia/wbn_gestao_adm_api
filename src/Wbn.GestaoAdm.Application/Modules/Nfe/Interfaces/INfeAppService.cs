using Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;

public interface INfeAppService
{
    Task AtualizarCertificadoDigitalAsync(AtualizarCertificadoDigitalEmpresaRequest request, CancellationToken cancellationToken = default);
    Task<BuscarNfeResponse> BuscarPorChaveAcessoAsync(BuscarNfeRequest request, CancellationToken cancellationToken = default);
    Task<SincronizarNfeResponse> SincronizarAsync(SincronizarNfeRequest request, CancellationToken cancellationToken = default);
    Task<NfeDocumentoResponse?> GetByChaveAcessoAsync(string chaveAcesso, ulong empresaId, CancellationToken cancellationToken = default);
    Task<string> GetXmlByChaveAcessoAsync(string chaveAcesso, ulong empresaId, CancellationToken cancellationToken = default);
    Task<List<NfeProdutoResponse>> GetProdutosByChaveAcessoAsync(string chaveAcesso, ulong empresaId, CancellationToken cancellationToken = default);
    Task<NfeDocumentoResponse> ManifestarAsync(string chaveAcesso, ManifestarNfeRequest request, CancellationToken cancellationToken = default);
}
