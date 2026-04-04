using System.ComponentModel.DataAnnotations.Schema;

namespace Wbn.GestaoAdm.Domain.Common.Entities;

public abstract class BaseEntity
{
    protected readonly List<string> ErrorsInternal = [];

    public ulong Id { get; set; }

    [NotMapped]
    public bool Deletado { get; private set; }

    [NotMapped]
    public IReadOnlyCollection<string> Errors => ErrorsInternal;

    [NotMapped]
    public bool HasErrors => ErrorsInternal.Count != 0;

    public abstract bool Validate();

    protected void AddError(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            ErrorsInternal.Add(message);
        }
    }

    public void ClearErrors()
    {
        ErrorsInternal.Clear();
    }

    public void MarkAsDeleted()
    {
        Deletado = true;
    }

    public void UnmarkAsDeleted()
    {
        Deletado = false;
    }
}
