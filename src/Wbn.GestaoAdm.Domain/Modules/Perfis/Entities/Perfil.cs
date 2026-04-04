using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Perfis.Entities;

public sealed class Perfil : AuditableEntity
{
    private readonly List<Usuario> _usuarios = [];

    private Perfil()
    {
    }

    public Perfil(string nome, string? descricao, bool ativo = true)
    {
        Nome = NormalizeRequired(nome);
        Descricao = NormalizeOptional(descricao);
        Ativo = ativo;
        DefinirDataCadastro();
    }

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Ativo { get; private set; }

    public IReadOnlyCollection<Usuario> Usuarios => _usuarios;

    public void Atualizar(string nome, string? descricao, bool ativo)
    {
        Nome = NormalizeRequired(nome);
        Descricao = NormalizeOptional(descricao);
        Ativo = ativo;
        DefinirDataAtualizacao();
    }

    public override bool Validate()
    {
        ClearErrors();

        if (string.IsNullOrWhiteSpace(Nome))
        {
            AddError("O nome do perfil e obrigatorio.");
        }

        return !HasErrors;
    }

    private static string NormalizeRequired(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
