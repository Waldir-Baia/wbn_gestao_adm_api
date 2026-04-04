using System.Net.Mail;
using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;

public sealed class Usuario : AuditableEntity
{
    private readonly List<UsuarioEmpresa> _usuariosEmpresas = [];
    private readonly List<Recebimento> _recebimentosEnviados = [];
    private readonly List<RecebimentoConferencia> _conferencias = [];
    private readonly List<RecebimentoDivergencia> _divergenciasRegistradas = [];
    private readonly List<RecebimentoDivergencia> _divergenciasResolvidas = [];
    private readonly List<RecebimentoHistorico> _historicos = [];
    private readonly List<RecebimentoComentario> _comentarios = [];

    private Usuario()
    {
    }

    public Usuario(
        ulong perfilId,
        string nome,
        string email,
        string login,
        string senhaHash,
        string? telefone,
        bool ativo = true)
    {
        PerfilId = perfilId;
        Nome = NormalizeRequired(nome);
        Email = NormalizeEmail(email);
        Login = NormalizeLogin(login);
        SenhaHash = NormalizeRequired(senhaHash);
        Telefone = NormalizeOptional(telefone);
        Ativo = ativo;
        DefinirDataCadastro();
    }

    public ulong PerfilId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Login { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime? UltimoLogin { get; private set; }

    public Perfil Perfil { get; private set; } = null!;

    public IReadOnlyCollection<UsuarioEmpresa> UsuariosEmpresas => _usuariosEmpresas;
    public IReadOnlyCollection<Recebimento> RecebimentosEnviados => _recebimentosEnviados;
    public IReadOnlyCollection<RecebimentoConferencia> Conferencias => _conferencias;
    public IReadOnlyCollection<RecebimentoDivergencia> DivergenciasRegistradas => _divergenciasRegistradas;
    public IReadOnlyCollection<RecebimentoDivergencia> DivergenciasResolvidas => _divergenciasResolvidas;
    public IReadOnlyCollection<RecebimentoHistorico> Historicos => _historicos;
    public IReadOnlyCollection<RecebimentoComentario> Comentarios => _comentarios;

    public void Atualizar(
        ulong perfilId,
        string nome,
        string email,
        string login,
        string senhaHash,
        string? telefone,
        bool ativo)
    {
        PerfilId = perfilId;
        Nome = NormalizeRequired(nome);
        Email = NormalizeEmail(email);
        Login = NormalizeLogin(login);
        SenhaHash = NormalizeRequired(senhaHash);
        Telefone = NormalizeOptional(telefone);
        Ativo = ativo;
        DefinirDataAtualizacao();
    }

    public void RegistrarUltimoLogin(DateTime? ultimoLogin = null)
    {
        UltimoLogin = ultimoLogin ?? DateTime.UtcNow;
        DefinirDataAtualizacao();
    }

    public override bool Validate()
    {
        ClearErrors();

        if (PerfilId == 0)
        {
            AddError("O perfil do usuario e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Nome))
        {
            AddError("O nome do usuario e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            AddError("O e-mail do usuario e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(Login))
        {
            AddError("O login do usuario e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(SenhaHash))
        {
            AddError("A senha hash do usuario e obrigatoria.");
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
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private static string NormalizeLogin(string login)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        return login.Trim().ToLowerInvariant();
    }

    private static string NormalizeEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return new MailAddress(email.Trim()).Address.ToLowerInvariant();
    }
}
