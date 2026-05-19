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
    private string UrlConsultaProtocolo => configuration["Sefaz:UrlConsultaProtocolo"]
        ?? "https://nfe.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx";

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

    // Mapeia código UF (primeiros 2 dígitos da chave) para o endpoint NFeConsultaProtocolo
    private static readonly Dictionary<int, (string Url, string WsdlNs)> UrlsConsultaPorUf = new()
    {
        { 12, ("https://nfe.fazenda.ac.gov.br/nfe2/services/NFeConsulta2",         "NFeConsulta2") },
        { 27, ("https://nfe.fazenda.al.gov.br/nfe/services/NFeConsulta2",          "NFeConsulta2") },
        { 13, ("https://nfe.sefaz.am.gov.br/services2/NfeConsulta2",              "NFeConsulta2") },
        { 16, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 29, ("https://nfe.sefaz.ba.gov.br/webservices/NfeConsulta2/NfeConsulta2.asmx", "NFeConsulta2") },
        { 23, ("https://nfece.sefaz.ce.gov.br/nfe2/services/NFeConsulta2",        "NFeConsulta2") },
        { 53, ("https://nfe.fazenda.df.gov.br/nfe2/services/NFeConsulta2",        "NFeConsulta2") },
        { 32, ("https://nfe.sefaz.es.gov.br/nfe2/services/NFeConsulta2",          "NFeConsulta2") },
        { 52, ("https://nfe.sefaz.go.gov.br/nfe/services/NFeConsulta2",           "NFeConsulta2") },
        { 21, ("https://www.sefazvirtual.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx", "NFeConsultaProtocolo4") },
        { 51, ("https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeConsulta2",      "NFeConsulta2") },
        { 50, ("https://nfe.sefaz.ms.gov.br/ws/NfeConsulta2",                     "NFeConsulta2") },
        { 31, ("https://nfe.fazenda.mg.gov.br/nfe2/services/NFeConsulta2",        "NFeConsulta2") },
        { 15, ("https://www.sefazvirtual.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx", "NFeConsultaProtocolo4") },
        { 25, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 41, ("https://nfe.sefaz.pr.gov.br/nfe/services/NFeConsulta2",           "NFeConsulta2") },
        { 26, ("https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeConsulta2",   "NFeConsulta2") },
        { 22, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 33, ("https://nfe.fazenda.rj.gov.br/nfe/services/NFeConsulta2",         "NFeConsulta2") },
        { 24, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 11, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 14, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 43, ("https://nfe.fazenda.rs.gov.br/nfe/services/NFeConsulta2",         "NFeConsulta2") },
        { 42, ("https://nfe.sef.sc.gov.br/nfe/services/NFeConsulta2",             "NFeConsulta2") },
        { 28, ("https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx",     "NFeConsulta2") },
        { 35, ("https://nfe.fazenda.sp.gov.br/nfe2/services/NFeConsulta2",        "NFeConsulta2") },
        { 17, ("https://nfe.sefaz.to.gov.br/nfeweb/services/NfeConsulta2",        "NFeConsulta2") },
    };

    public async Task<string?> ConsultarProtocoloAsync(
        X509Certificate2 certificado,
        string chaveAcesso,
        CancellationToken cancellationToken = default)
    {
        int.TryParse(chaveAcesso.Length >= 2 ? chaveAcesso[..2] : "0", out var ufCodigo);
        var (url, wsdlSufixo) = UrlsConsultaPorUf.TryGetValue(ufCodigo, out var entry)
            ? entry
            : (UrlConsultaProtocolo, "NFeConsultaProtocolo4");

        var wsdlNs = $"http://www.portalfiscal.inf.br/nfe/wsdl/{wsdlSufixo}";

        var xmlBody = $"""
                       <consSitNFe xmlns="http://www.portalfiscal.inf.br/nfe" versao="4.01">
                         <tpAmb>{TipoAmbiente}</tpAmb>
                         <xServ>CONSULTAR</xServ>
                         <chNFe>{chaveAcesso}</chNFe>
                       </consSitNFe>
                       """;

        var soapEnvelope = $"""
                            <?xml version="1.0" encoding="utf-8"?>
                            <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                             xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                                             xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
                              <soap12:Body>
                                <nfeConsultaNF xmlns="{wsdlNs}">
                                  <nfeDadosMsg>
                                    {xmlBody}
                                  </nfeDadosMsg>
                                </nfeConsultaNF>
                              </soap12:Body>
                            </soap12:Envelope>
                            """;

        string respostaXml;
        try
        {
            respostaXml = await EnviarSoapAsync(
                url,
                soapEnvelope,
                $"{wsdlNs}/nfeConsultaNF",
                certificado,
                cancellationToken);
        }
        catch
        {
            return null;
        }

        XDocument doc;
        try { doc = XDocument.Parse(respostaXml); }
        catch { return null; }

        var retConsSit = doc.Descendants(NfeNs + "retConsSitNFe").FirstOrDefault();
        var cStat = retConsSit?.Element(NfeNs + "cStat")?.Value;

        if (cStat is not ("100" or "101")) return null;

        var procNFe = retConsSit?.Descendants(NfeNs + "procNFe").FirstOrDefault()
                   ?? retConsSit?.Descendants("procNFe").FirstOrDefault();

        return procNFe?.ToString();
    }

    public async Task<SefazDistribuicaoResult> ConsultarPorChaveAcessoAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        string chaveAcesso,
        CancellationToken cancellationToken = default)
    {
        var xmlBody = MontarXmlConsultaChave(cnpj, codigoUf, chaveAcesso);
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
            "http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4/nfeRecepcaoEventoNF",
            certificado,
            cancellationToken);

        return ParsearRespostaManifestacao(respostaXml);
    }

    private string MontarXmlConsultaChave(string cnpj, int codigoUf, string chaveAcesso)
    {
        var ufFormatado = codigoUf == 0 ? "91" : codigoUf.ToString();

        return $"""
                <distDFeInt xmlns="http://www.portalfiscal.inf.br/nfe" versao="1.01">
                  <tpAmb>{TipoAmbiente}</tpAmb>
                  <cUFAutor>{ufFormatado}</cUFAutor>
                  <CNPJ>{cnpj}</CNPJ>
                  <consChNFe>
                    <chNFe>{chaveAcesso}</chNFe>
                  </consChNFe>
                </distDFeInt>
                """;
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

        var cOrgao = "91";
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
        signedXml.SignedInfo!.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

        var reference = new Reference("#" + referenceId);
        reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
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
                    <nfeDadosMsg xmlns="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4">
                      <envEvento xmlns="http://www.portalfiscal.inf.br/nfe" versao="1.00">
                        <idLote>000000000000001</idLote>
                        {xmlEvento}
                      </envEvento>
                    </nfeDadosMsg>
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
        content.Headers.ContentType = MediaTypeHeaderValue.Parse(
            $"application/soap+xml; charset=utf-8; action=\"{soapAction}\"");
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
        var retEnvEvento = doc.Descendants(NfeNs + "retEnvEvento").FirstOrDefault();

        var cStat = retEvento?.Element(NfeNs + "cStat")?.Value
            ?? retEnvEvento?.Element(NfeNs + "cStat")?.Value
            ?? string.Empty;
        var xMotivo = retEvento?.Element(NfeNs + "xMotivo")?.Value
            ?? retEnvEvento?.Element(NfeNs + "xMotivo")?.Value
            ?? string.Empty;
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
