using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;

public sealed class UsuarioEmpresa : BaseEntity
{
    private UsuarioEmpresa()
    {
    }

    public UsuarioEmpresa(ulong usuarioId, ulong empresaId, bool ativo = true)
    {
        UsuarioId = usuarioId;
        EmpresaId = empresaId;
        Ativo = ativo;
        DataCadastro = DateTime.UtcNow;
    }

    public ulong UsuarioId { get; private set; }
    public ulong EmpresaId { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public Usuario Usuario { get; private set; } = null!;
    public Empresa Empresa { get; private set; } = null!;

    public void AtualizarStatus(bool ativo)
    {
        Ativo = ativo;
    }

    public override bool Validate()
    {
        ClearErrors();

        if (UsuarioId == 0)
        {
            AddError("O usuario do vinculo e obrigatorio.");
        }

        if (EmpresaId == 0)
        {
            AddError("A empresa do vinculo e obrigatoria.");
        }

        return !HasErrors;
    }
}
