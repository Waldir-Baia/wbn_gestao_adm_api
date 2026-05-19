using System.Security.Cryptography.X509Certificates;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Enums;

namespace Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;

public sealed record SefazDocumento(
    long Nsu,
    string Schema,
    TipoDocumentoFiscalEnum TipoDocumento,
    string XmlDescomprimido);

public sealed record SefazDistribuicaoResult(
    string CodigoStatus,
    string Motivo,
    long UltimoNsu,
    long MaxNsu,
    IReadOnlyList<SefazDocumento> Documentos);

public sealed record SefazManifestacaoResult(
    string CodigoStatus,
    string Motivo,
    string? Protocolo,
    string XmlRetorno);

public interface ISefazNfeClient
{
    Task<SefazDistribuicaoResult> ConsultarDistribuicaoDFeAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        long ultimoNsu,
        CancellationToken cancellationToken = default);

    Task<SefazDistribuicaoResult> ConsultarPorChaveAcessoAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        string chaveAcesso,
        CancellationToken cancellationToken = default);

    Task<string?> ConsultarProtocoloAsync(
        X509Certificate2 certificado,
        string chaveAcesso,
        CancellationToken cancellationToken = default);

    Task<SefazManifestacaoResult> EnviarManifestacaoAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        string chaveAcesso,
        TipoManifestacaoNfeEnum tipoManifestacao,
        string? justificativa,
        CancellationToken cancellationToken = default);
}
