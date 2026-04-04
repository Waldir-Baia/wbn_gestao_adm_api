namespace Wbn.GestaoAdm.Domain.Common.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime DataCadastro { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    protected void DefinirDataCadastro(DateTime? dataCadastro = null)
    {
        DataCadastro = dataCadastro ?? DateTime.UtcNow;
    }

    protected void DefinirDataAtualizacao(DateTime? dataAtualizacao = null)
    {
        DataAtualizacao = dataAtualizacao ?? DateTime.UtcNow;
    }
}
