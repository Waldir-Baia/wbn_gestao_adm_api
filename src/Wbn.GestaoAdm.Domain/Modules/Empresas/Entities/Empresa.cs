using System.Net.Mail;
using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.UsuariosEmpresas.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;

public sealed class Empresa : AuditableEntity
{
    private readonly List<UsuarioEmpresa> _usuariosEmpresas = [];
    private readonly List<Recebimento> _recebimentos = [];

    private Empresa()
    {
    }

    public Empresa(
        string nomeFantasia,
        string razaoSocial,
        string cnpj,
        string? codigoInterno,
        string email,
        string? telefone,
        bool ativo = true)
    {
        NomeFantasia = NormalizeRequired(nomeFantasia);
        RazaoSocial = NormalizeRequired(razaoSocial);
        Cnpj = NormalizeDigits(cnpj);
        CodigoInterno = NormalizeOptional(codigoInterno);
        Email = NormalizeEmail(email);
        Telefone = NormalizeOptional(telefone);
        Ativo = ativo;
        DefinirDataCadastro();
    }

    public string NomeFantasia { get; private set; } = string.Empty;
    public string RazaoSocial { get; private set; } = string.Empty;
    public string Cnpj { get; private set; } = string.Empty;
    public string? CodigoInterno { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public bool Ativo { get; private set; }

    public IReadOnlyCollection<UsuarioEmpresa> UsuariosEmpresas => _usuariosEmpresas;
    public IReadOnlyCollection<Recebimento> Recebimentos => _recebimentos;

    public void Atualizar(
        string nomeFantasia,
        string razaoSocial,
        string cnpj,
        string? codigoInterno,
        string email,
        string? telefone,
        bool ativo)
    {
        NomeFantasia = NormalizeRequired(nomeFantasia);
        RazaoSocial = NormalizeRequired(razaoSocial);
        Cnpj = NormalizeDigits(cnpj);
        CodigoInterno = NormalizeOptional(codigoInterno);
        Email = NormalizeEmail(email);
        Telefone = NormalizeOptional(telefone);
        Ativo = ativo;
        DefinirDataAtualizacao();
    }

    public override bool Validate()
    {
        ClearErrors();

        if (Cnpj.Length != 14)
        {
            AddError("O CNPJ da empresa deve conter 14 digitos.");
        }

        if (string.IsNullOrWhiteSpace(NomeFantasia))
        {
            AddError("O nome fantasia da empresa e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(RazaoSocial))
        {
            AddError("A razao social da empresa e obrigatoria.");
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

    private static string NormalizeEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return new MailAddress(email.Trim()).Address.ToLowerInvariant();
    }

    private static string NormalizeDigits(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
