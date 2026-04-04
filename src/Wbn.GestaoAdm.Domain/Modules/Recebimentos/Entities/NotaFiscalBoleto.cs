using Wbn.GestaoAdm.Domain.Common.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;

public sealed class NotaFiscalBoleto : BaseEntity
{
    private NotaFiscalBoleto()
    {
    }

    public NotaFiscalBoleto(ulong notaFiscalId, ulong boletoId)
    {
        NotaFiscalId = notaFiscalId;
        BoletoId = boletoId;
        DataCadastro = DateTime.UtcNow;
    }

    public ulong NotaFiscalId { get; private set; }
    public ulong BoletoId { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public NotaFiscal NotaFiscal { get; private set; } = null!;
    public Boleto Boleto { get; private set; } = null!;

    public override bool Validate()
    {
        ClearErrors();

        if (NotaFiscalId == 0)
        {
            AddError("A nota fiscal do vinculo e obrigatoria.");
        }

        if (BoletoId == 0)
        {
            AddError("O boleto do vinculo e obrigatorio.");
        }

        return !HasErrors;
    }
}
