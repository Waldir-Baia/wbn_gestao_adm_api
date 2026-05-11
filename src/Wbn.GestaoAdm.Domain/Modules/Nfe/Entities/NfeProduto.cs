using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;

public sealed class NfeProduto : AuditableEntity
{
    private NfeProduto()
    {
    }

    public NfeProduto(
        ulong nfeDocumentoId,
        string nomeProduto,
        string? codigoProduto,
        string? descricao,
        string? ncm,
        string? cfop,
        string? unidade,
        decimal? quantidade,
        decimal? valorUnitario,
        decimal? valorTotal,
        string? ean)
    {
        NfeDocumentoId = nfeDocumentoId;
        NomeProduto = nomeProduto.Trim();
        CodigoProduto = string.IsNullOrWhiteSpace(codigoProduto) ? null : codigoProduto.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Ncm = string.IsNullOrWhiteSpace(ncm) ? null : ncm.Trim();
        Cfop = string.IsNullOrWhiteSpace(cfop) ? null : cfop.Trim();
        Unidade = string.IsNullOrWhiteSpace(unidade) ? null : unidade.Trim();
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = valorTotal;
        Ean = string.IsNullOrWhiteSpace(ean) ? null : ean.Trim();
        DefinirDataCadastro();
    }

    public ulong NfeDocumentoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public string? CodigoProduto { get; private set; }
    public string? Descricao { get; private set; }
    public string? Ncm { get; private set; }
    public string? Cfop { get; private set; }
    public string? Unidade { get; private set; }
    public decimal? Quantidade { get; private set; }
    public decimal? ValorUnitario { get; private set; }
    public decimal? ValorTotal { get; private set; }
    public string? Ean { get; private set; }

    public NfeDocumento NfeDocumento { get; private set; } = null!;

    public override bool Validate()
    {
        ClearErrors();

        if (NfeDocumentoId == 0)
        {
            AddError("O documento NF-e do produto e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(NomeProduto))
        {
            AddError("O nome do produto e obrigatorio.");
        }

        return !HasErrors;
    }
}
