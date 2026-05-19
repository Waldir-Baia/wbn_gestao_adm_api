using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Enums;

namespace Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;

public sealed class NfeDocumento : AuditableEntity
{
    private readonly List<NfeProduto> _produtos = [];

    private NfeDocumento()
    {
    }

    public NfeDocumento(
        ulong empresaId,
        string chaveAcesso,
        long? nsu,
        TipoDocumentoFiscalEnum tipoDocumento,
        string? schemaDocumento,
        string? xmlResumo,
        string? xmlCompleto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chaveAcesso);

        EmpresaId = empresaId;
        ChaveAcesso = chaveAcesso.Trim();
        Nsu = nsu;
        TipoDocumento = tipoDocumento;
        SchemaDocumento = string.IsNullOrWhiteSpace(schemaDocumento) ? null : schemaDocumento.Trim();
        XmlResumo = xmlResumo;
        XmlCompleto = xmlCompleto;
        StatusManifestacao = StatusManifestacaoNfeEnum.Pendente;

        if (xmlCompleto is not null)
        {
            DataDownload = DateTime.UtcNow;
        }

        DefinirDataCadastro();
    }

    public ulong EmpresaId { get; private set; }
    public string ChaveAcesso { get; private set; } = string.Empty;
    public long? Nsu { get; private set; }
    public TipoDocumentoFiscalEnum TipoDocumento { get; private set; }
    public string? SchemaDocumento { get; private set; }
    public string? CnpjEmitente { get; private set; }
    public string? NomeEmitente { get; private set; }
    public string? CnpjDestinatario { get; private set; }
    public string? NomeDestinatario { get; private set; }
    public string? NumeroNota { get; private set; }
    public string? Serie { get; private set; }
    public DateTime? DataEmissao { get; private set; }
    public decimal? ValorTotal { get; private set; }
    public StatusManifestacaoNfeEnum StatusManifestacao { get; private set; }
    public string? XmlResumo { get; private set; }
    public string? XmlCompleto { get; private set; }
    public string? XmlEvento { get; private set; }
    public string? ProtocoloManifestacao { get; private set; }
    public string? RetornoSefaz { get; private set; }
    public DateTime? DataDownload { get; private set; }

    public Empresa Empresa { get; private set; } = null!;
    public IReadOnlyCollection<NfeProduto> Produtos => _produtos;

    public void AtualizarDadosNota(
        string? cnpjEmitente,
        string? nomeEmitente,
        string? cnpjDestinatario,
        string? nomeDestinatario,
        string? numeroNota,
        string? serie,
        DateTime? dataEmissao,
        decimal? valorTotal)
    {
        CnpjEmitente = string.IsNullOrWhiteSpace(cnpjEmitente) ? null : cnpjEmitente.Trim();
        NomeEmitente = string.IsNullOrWhiteSpace(nomeEmitente) ? null : nomeEmitente.Trim();
        CnpjDestinatario = string.IsNullOrWhiteSpace(cnpjDestinatario) ? null : cnpjDestinatario.Trim();
        NomeDestinatario = string.IsNullOrWhiteSpace(nomeDestinatario) ? null : nomeDestinatario.Trim();
        NumeroNota = string.IsNullOrWhiteSpace(numeroNota) ? null : numeroNota.Trim();
        Serie = string.IsNullOrWhiteSpace(serie) ? null : serie.Trim();
        DataEmissao = dataEmissao;
        ValorTotal = valorTotal;
        DefinirDataAtualizacao();
    }

    public void SalvarXmlCompleto(string xmlCompleto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlCompleto);

        XmlCompleto = xmlCompleto;
        TipoDocumento = TipoDocumentoFiscalEnum.NfeCompleta;
        DataDownload = DateTime.UtcNow;
        DefinirDataAtualizacao();
    }

    public void RegistrarManifestacao(
        StatusManifestacaoNfeEnum novoStatus,
        string? protocolo,
        string? retornoSefaz)
    {
        if (StatusManifestacao == StatusManifestacaoNfeEnum.ConfirmacaoOperacao
            || StatusManifestacao == StatusManifestacaoNfeEnum.DesconhecimentoOperacao
            || StatusManifestacao == StatusManifestacaoNfeEnum.OperacaoNaoRealizada)
        {
            throw new RegraDeNegocioException(
                $"Nao e possivel alterar a manifestacao desta nota. Status atual: {StatusManifestacao}.");
        }

        StatusManifestacao = novoStatus;
        ProtocoloManifestacao = string.IsNullOrWhiteSpace(protocolo) ? null : protocolo.Trim();
        RetornoSefaz = retornoSefaz;
        DefinirDataAtualizacao();
    }

    public void SalvarXmlEvento(string xmlEvento)
    {
        XmlEvento = xmlEvento;
        DefinirDataAtualizacao();
    }

    public override bool Validate()
    {
        ClearErrors();

        if (EmpresaId == 0)
        {
            AddError("A empresa do documento NF-e e obrigatoria.");
        }

        if (string.IsNullOrWhiteSpace(ChaveAcesso))
        {
            AddError("A chave de acesso do documento NF-e e obrigatoria.");
        }
        else if (ChaveAcesso.Length != 44)
        {
            AddError("A chave de acesso do documento NF-e deve conter 44 digitos.");
        }

        return !HasErrors;
    }
}
