using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Enums;

namespace Wbn.GestaoAdm.Infrastructure.Sefaz;

public sealed class SefazNfeClient(IConfiguration configuration) : ISefazNfeClient
{
    private static readonly XNamespace NfeWsdlNs = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe";
    private static readonly XNamespace NfeNs = "http://www.portalfiscal.inf.br/nfe";
    private static readonly XNamespace EventoWsdlNs = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4";
    private static readonly XNamespace Soap12Ns = "http://www.w3.org/2003/05/soap-envelope";

    private string TipoAmbiente => configuration["Sefaz:TipoAmbiente"] ?? "1";
    private string UrlDistribuicaoDFe => configuration["Sefaz:UrlDistribuicaoDFe"]
        ?? "https://www1.nfe.fazenda.gov.br/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx";
    private string UrlRecepcaoEvento => configuration["Sefaz:UrlRecepcaoEvento"]
        ?? "https://www.nfe.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx";

    public async Task<SefazDistribuicaoResult> ConsultarDistribuicaoDFeAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        long ultimoNsu,
        CancellationToken cancellationToken = default)
    {
        var xmlBody = MontarXmlDistribuicaoDFe(cnpj, codigoUf, ultimoNsu);
        var soapEnvelope = MontarSoapEnvelopeDistribuicao(xmlBody);

        var respostaXml = await EnviarSoapAsync(
            UrlDistribuicaoDFe,
            soapEnvelope,
            "http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe/nfeDistDFeInteresse",
            certificado,
            cancellationToken);

        return ParsearRespostaDistribuicao(respostaXml);
    }

    public async Task<SefazManifestacaoResult> EnviarManifestacaoAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        string chaveAcesso,
        TipoManifestacaoNfeEnum tipoManifestacao,
        string? justificativa,
        CancellationToken cancellationToken = default)
    {
        var xmlEvento = MontarXmlEvento(cnpj, codigoUf, chaveAcesso, tipoManifestacao, justificativa, certificado);
        var soapEnvelope = MontarSoapEnvelopeEvento(xmlEvento);

        var respostaXml = await EnviarSoapAsync(
            UrlRecepcaoEvento,
            soapEnvelope,
            "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4/nfeRecepcaoEvento",
            certificado,
            cancellationToken);

        return ParsearRespostaManifestacao(respostaXml);
    }

    private string MontarXmlDistribuicaoDFe(string cnpj, int codigoUf, long ultimoNsu)
    {
        var nsuFormatado = ultimoNsu.ToString("D15");
        var ufFormatado = codigoUf == 0 ? "91" : codigoUf.ToString();

        return $"""
                <distDFeInt xmlns="http://www.portalfiscal.inf.br/nfe" versao="1.01">
                  <tpAmb>{TipoAmbiente}</tpAmb>
                  <cUFAutor>{ufFormatado}</cUFAutor>
                  <CNPJ>{cnpj}</CNPJ>
                  <distNSU>
                    <ultNSU>{nsuFormatado}</ultNSU>
                  </distNSU>
                </distDFeInt>
                """;
    }

    private static string MontarSoapEnvelopeDistribuicao(string xmlBody)
    {
        return $"""
                <?xml version="1.0" encoding="utf-8"?>
                <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                 xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                  <soap12:Body>
                    <nfeDistDFeInteresse xmlns="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe">
                      <nfeDadosMsg>
                        {xmlBody}
                      </nfeDadosMsg>
                    </nfeDistDFeInteresse>
                  </soap12:Body>
                </soap12:Envelope>
                """;
    }

    private string MontarXmlEvento(
        string cnpj,
        int codigoUf,
        string chaveAcesso,
        TipoManifestacaoNfeEnum tipoManifestacao,
        string? justificativa,
        X509Certificate2 certificado)
    {
        var tpEvento = (int)tipoManifestacao;
        var descEvento = tipoManifestacao switch
        {
            TipoManifestacaoNfeEnum.CienciaOperacao => "Ciencia da Operacao",
            TipoManifestacaoNfeEnum.ConfirmacaoOperacao => "Confirmacao da Operacao",
            TipoManifestacaoNfeEnum.DesconhecimentoOperacao => "Desconhecimento da Operacao",
            TipoManifestacaoNfeEnum.OperacaoNaoRealizada => "Operacao nao Realizada",
            _ => throw new InvalidOperationException("Tipo de manifestacao invalido.")
        };

        var cOrgao = codigoUf == 0 ? "91" : codigoUf.ToString();
        var dhEvento = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
        var idEvento = $"ID{tpEvento}{chaveAcesso}01";

        var detEvento = tipoManifestacao == TipoManifestacaoNfeEnum.OperacaoNaoRealizada && !string.IsNullOrWhiteSpace(justificativa)
            ? $"""
               <detEvento versao="1.00">
                 <descEvento>{descEvento}</descEvento>
                 <xJust>{System.Security.SecurityElement.Escape(justificativa)}</xJust>
               </detEvento>
               """
            : $"""
               <detEvento versao="1.00">
                 <descEvento>{descEvento}</descEvento>
               </detEvento>
               """;

        var xmlInfEvento = $"""
                            <infEvento Id="{idEvento}">
                              <cOrgao>{cOrgao}</cOrgao>
                              <tpAmb>{TipoAmbiente}</tpAmb>
                              <CNPJ>{cnpj}</CNPJ>
                              <chNFe>{chaveAcesso}</chNFe>
                              <dhEvento>{dhEvento}</dhEvento>
                              <tpEvento>{tpEvento}</tpEvento>
                              <nSeqEvento>1</nSeqEvento>
                              <verEvento>1.00</verEvento>
                              {detEvento}
                            </infEvento>
                            """;

        var xmlDoc = new XmlDocument();
        var eventoXml = $"""
                         <evento xmlns="http://www.portalfiscal.inf.br/nfe" versao="1.00">
                           {xmlInfEvento}
                         </evento>
                         """;
        xmlDoc.LoadXml(eventoXml);

        var xmlAssinado = AssinarXml(xmlDoc, idEvento, certificado);

        return xmlAssinado;
    }

    private static string AssinarXml(XmlDocument xmlDoc, string referenceId, X509Certificate2 certificado)
    {
        var signedXml = new SignedXml(xmlDoc)
        {
            SigningKey = certificado.GetRSAPrivateKey()
        };

        var reference = new Reference("#" + referenceId);
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        reference.AddTransform(new XmlDsigC14NTransform());
        signedXml.AddReference(reference);

        var keyInfo = new KeyInfo();
        keyInfo.AddClause(new KeyInfoX509Data(certificado));
        signedXml.KeyInfo = keyInfo;

        signedXml.ComputeSignature();
        var xmlSignature = signedXml.GetXml();

        xmlDoc.DocumentElement!.AppendChild(xmlDoc.ImportNode(xmlSignature, true));

        return xmlDoc.OuterXml;
    }

    private static string MontarSoapEnvelopeEvento(string xmlEvento)
    {
        return $"""
                <?xml version="1.0" encoding="utf-8"?>
                <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                 xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                 xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                  <soap12:Body>
                    <nfeRecepcaoEvento xmlns="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4">
                      <nfeDadosMsg>
                        <envEvento xmlns="http://www.portalfiscal.inf.br/nfe" versao="1.00">
                          <idLote>1</idLote>
                          {xmlEvento}
                        </envEvento>
                      </nfeDadosMsg>
                    </nfeRecepcaoEvento>
                  </soap12:Body>
                </soap12:Envelope>
                """;
    }

    private static async Task<string> EnviarSoapAsync(
        string url,
        string soapEnvelope,
        string soapAction,
        X509Certificate2 certificado,
        CancellationToken cancellationToken)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificado);
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        using var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };

        var content = new StringContent(soapEnvelope, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/soap+xml")
        {
            CharSet = "utf-8"
        };
        content.Headers.TryAddWithoutValidation("SOAPAction", soapAction);

        HttpResponseMessage resposta;
        try
        {
            resposta = await httpClient.PostAsync(url, content, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Falha de comunicacao com a SEFAZ: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException("Timeout na comunicacao com a SEFAZ.", ex);
        }

        var xmlResposta = await resposta.Content.ReadAsStringAsync(cancellationToken);
        return xmlResposta;
    }

    private static SefazDistribuicaoResult ParsearRespostaDistribuicao(string xmlResposta)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Parse(xmlResposta);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Resposta invalida da SEFAZ: {ex.Message}", ex);
        }

        var retDist = doc.Descendants(NfeNs + "retDistDFeInt").FirstOrDefault()
            ?? throw new InvalidOperationException("Elemento retDistDFeInt nao encontrado na resposta da SEFAZ.");

        var cStat = retDist.Element(NfeNs + "cStat")?.Value ?? string.Empty;
        var xMotivo = retDist.Element(NfeNs + "xMotivo")?.Value ?? string.Empty;
        var ultNsuStr = retDist.Element(NfeNs + "ultNSU")?.Value ?? "0";
        var maxNsuStr = retDist.Element(NfeNs + "maxNSU")?.Value ?? "0";

        long.TryParse(ultNsuStr, out var ultNsu);
        long.TryParse(maxNsuStr, out var maxNsu);

        var documentos = new List<SefazDocumento>();

        foreach (var docZip in retDist.Descendants(NfeNs + "docZip"))
        {
            var nsuAttr = docZip.Attribute("NSU")?.Value ?? "0";
            var schema = docZip.Attribute("schema")?.Value ?? string.Empty;
            var conteudoBase64 = docZip.Value;

            long.TryParse(nsuAttr, out var nsu);

            string xmlDescomprimido;
            try
            {
                xmlDescomprimido = DescomprimirBase64Gzip(conteudoBase64);
            }
            catch
            {
                continue;
            }

            var tipoDoc = NfeXmlParserHelper.ResolverTipo(schema);

            documentos.Add(new SefazDocumento(nsu, schema, tipoDoc, xmlDescomprimido));
        }

        return new SefazDistribuicaoResult(cStat, xMotivo, ultNsu, maxNsu, documentos);
    }

    private static SefazManifestacaoResult ParsearRespostaManifestacao(string xmlResposta)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Parse(xmlResposta);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Resposta invalida da SEFAZ: {ex.Message}", ex);
        }

        var retEvento = doc.Descendants(NfeNs + "infEvento").FirstOrDefault();

        var cStat = retEvento?.Element(NfeNs + "cStat")?.Value ?? string.Empty;
        var xMotivo = retEvento?.Element(NfeNs + "xMotivo")?.Value ?? string.Empty;
        var nProt = retEvento?.Element(NfeNs + "nProt")?.Value;

        return new SefazManifestacaoResult(cStat, xMotivo, nProt, xmlResposta);
    }

    private static string DescomprimirBase64Gzip(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        using var inputStream = new MemoryStream(bytes);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        using var outputStream = new MemoryStream();
        gzipStream.CopyTo(outputStream);
        return Encoding.UTF8.GetString(outputStream.ToArray());
    }

    private static class NfeXmlParserHelper
    {
        public static TipoDocumentoFiscalEnum ResolverTipo(string schema)
        {
            return schema switch
            {
                var s when s.StartsWith("resNFe", StringComparison.OrdinalIgnoreCase) => TipoDocumentoFiscalEnum.ResumoNfe,
                var s when s.StartsWith("procNFe", StringComparison.OrdinalIgnoreCase) => TipoDocumentoFiscalEnum.NfeCompleta,
                var s when s.StartsWith("resEvento", StringComparison.OrdinalIgnoreCase) => TipoDocumentoFiscalEnum.ResumoEvento,
                var s when s.StartsWith("procEventoNFe", StringComparison.OrdinalIgnoreCase) => TipoDocumentoFiscalEnum.EventoNfe,
                var s when s.StartsWith("resEvCancNFe", StringComparison.OrdinalIgnoreCase) => TipoDocumentoFiscalEnum.ResumoCancelamento,
                _ => TipoDocumentoFiscalEnum.Outro
            };
        }
    }
}
