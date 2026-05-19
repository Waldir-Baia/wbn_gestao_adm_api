using System.Xml.Linq;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Enums;

namespace Wbn.GestaoAdm.Application.Modules.Nfe.Services;

public sealed record NfeProdutoXml(
    string? CodigoProduto,
    string NomeProduto,
    string? Ncm,
    string? Cfop,
    string? Unidade,
    decimal? Quantidade,
    decimal? ValorUnitario,
    decimal? ValorTotal,
    string? Ean);

public sealed record NfeDadosResumo(
    string? CnpjEmitente,
    string? NomeEmitente,
    string? CnpjDestinatario,
    DateTime? DataEmissao,
    decimal? ValorTotal);

public sealed record NfeDadosXml(
    string ChaveAcesso,
    string? NumeroNota,
    string? Serie,
    DateTime? DataEmissao,
    string? CnpjEmitente,
    string? NomeEmitente,
    string? CnpjDestinatario,
    string? NomeDestinatario,
    decimal? ValorTotal,
    IReadOnlyList<NfeProdutoXml> Produtos);

public static class NfeXmlParser
{
    private static readonly XNamespace NfeNs = "http://www.portalfiscal.inf.br/nfe";

    public static NfeDadosXml? ParsearNfeCompleta(string xmlCompleto)
    {
        if (string.IsNullOrWhiteSpace(xmlCompleto))
        {
            return null;
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xmlCompleto);
        }
        catch
        {
            return null;
        }

        var raiz = doc.Root;
        var infNfe = raiz?.Descendants(NfeNs + "infNFe").FirstOrDefault();

        if (infNfe is null)
        {
            return null;
        }

        var ide = infNfe.Element(NfeNs + "ide");
        var emit = infNfe.Element(NfeNs + "emit");
        var dest = infNfe.Element(NfeNs + "dest");
        var total = infNfe.Element(NfeNs + "total")?.Element(NfeNs + "ICMSTot");

        var chaveAcesso = infNfe.Attribute("Id")?.Value?.Replace("NFe", string.Empty) ?? string.Empty;
        var numeroNota = ide?.Element(NfeNs + "nNF")?.Value;
        var serie = ide?.Element(NfeNs + "serie")?.Value;
        var dataEmissaoStr = ide?.Element(NfeNs + "dhEmi")?.Value ?? ide?.Element(NfeNs + "dEmi")?.Value;
        DateTime? dataEmissao = DateTime.TryParse(dataEmissaoStr, out var dtEmissao) ? dtEmissao : null;

        var cnpjEmitente = emit?.Element(NfeNs + "CNPJ")?.Value;
        var nomeEmitente = emit?.Element(NfeNs + "xFant")?.Value ?? emit?.Element(NfeNs + "xNome")?.Value;
        var cnpjDestinatario = dest?.Element(NfeNs + "CNPJ")?.Value ?? dest?.Element(NfeNs + "CPF")?.Value;
        var nomeDestinatario = dest?.Element(NfeNs + "xNome")?.Value;

        decimal? valorTotal = ParseDecimal(total?.Element(NfeNs + "vNF")?.Value);

        var produtos = ParsearProdutos(infNfe);

        return new NfeDadosXml(
            chaveAcesso,
            numeroNota,
            serie,
            dataEmissao,
            cnpjEmitente,
            nomeEmitente,
            cnpjDestinatario,
            nomeDestinatario,
            valorTotal,
            produtos);
    }

    private static List<NfeProdutoXml> ParsearProdutos(XElement infNfe)
    {
        var produtos = new List<NfeProdutoXml>();

        foreach (var det in infNfe.Elements(NfeNs + "det"))
        {
            var prod = det.Element(NfeNs + "prod");
            if (prod is null) continue;

            var nomeProduto = prod.Element(NfeNs + "xProd")?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(nomeProduto)) continue;

            var ean = prod.Element(NfeNs + "cEAN")?.Value;
            if (ean is "SEM GTIN" or "000000000000000") ean = null;

            produtos.Add(new NfeProdutoXml(
                CodigoProduto: prod.Element(NfeNs + "cProd")?.Value,
                NomeProduto: nomeProduto.Trim(),
                Ncm: prod.Element(NfeNs + "NCM")?.Value,
                Cfop: prod.Element(NfeNs + "CFOP")?.Value,
                Unidade: prod.Element(NfeNs + "uCom")?.Value ?? prod.Element(NfeNs + "uTrib")?.Value,
                Quantidade: ParseDecimal(prod.Element(NfeNs + "qCom")?.Value),
                ValorUnitario: ParseDecimal(prod.Element(NfeNs + "vUnCom")?.Value),
                ValorTotal: ParseDecimal(prod.Element(NfeNs + "vProd")?.Value),
                Ean: ean));
        }

        return produtos;
    }

    public static TipoDocumentoFiscalEnum ResolverTipoDocumentoPorSchema(string? schema)
    {
        if (string.IsNullOrWhiteSpace(schema))
        {
            return TipoDocumentoFiscalEnum.Outro;
        }

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

    public static string? ExtrairChaveAcessoDoResumo(string xmlResumo)
    {
        if (string.IsNullOrWhiteSpace(xmlResumo)) return null;

        try
        {
            var doc = XDocument.Parse(xmlResumo);
            return doc.Descendants(NfeNs + "chNFe").FirstOrDefault()?.Value
                ?? doc.Descendants("chNFe").FirstOrDefault()?.Value;
        }
        catch
        {
            return null;
        }
    }

    public static NfeDadosResumo? ParsearResumoNfe(string xmlResumo)
    {
        if (string.IsNullOrWhiteSpace(xmlResumo)) return null;

        XDocument doc;
        try { doc = XDocument.Parse(xmlResumo); }
        catch { return null; }

        var resNFe = doc.Descendants(NfeNs + "resNFe").FirstOrDefault()
                  ?? doc.Descendants("resNFe").FirstOrDefault()
                  ?? doc.Root;

        if (resNFe is null) return null;

        XElement? El(string local) =>
            resNFe.Element(NfeNs + local) ?? resNFe.Element(local);

        var cnpjEmitente = El("CNPJ")?.Value;
        var nomeEmitente = El("xNome")?.Value;

        var dest = resNFe.Element(NfeNs + "dest") ?? resNFe.Element("dest");
        var cnpjDest = dest?.Element(NfeNs + "CNPJ")?.Value
                    ?? dest?.Element(NfeNs + "CPF")?.Value
                    ?? dest?.Element("CNPJ")?.Value
                    ?? dest?.Element("CPF")?.Value;

        var dhEmi = El("dhEmi")?.Value ?? El("dEmi")?.Value;
        DateTime? dataEmissao = DateTime.TryParse(dhEmi, out var dt) ? dt : null;

        decimal? valorTotal = ParseDecimal(El("vNF")?.Value);

        return new NfeDadosResumo(cnpjEmitente, nomeEmitente, cnpjDest, dataEmissao, valorTotal);
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return decimal.TryParse(value, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : null;
    }
}
