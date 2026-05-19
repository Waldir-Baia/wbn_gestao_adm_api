using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;

namespace Wbn.GestaoAdm.Infrastructure.Sefaz;

public sealed class CertificadoDigitalProvider(IConfiguration configuration) : ICertificadoDigitalProvider
{
    private byte[] ChaveCriptografia
    {
        get
        {
            var segredo = configuration["JWT_SECRET_KEY"]
                ?? throw new InvalidOperationException("JWT_SECRET_KEY nao configurada para derivar chave de criptografia.");
            return SHA256.HashData(Encoding.UTF8.GetBytes(segredo));
        }
    }

    public X509Certificate2 CarregarCertificado(byte[] bytes, string senhaDecriptografada)
    {
        try
        {
            return new X509Certificate2(bytes, senhaDecriptografada, ResolverFlags());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Nao foi possivel carregar o certificado digital. Verifique se o arquivo e a senha estao corretos.", ex);
        }
    }

    public string CriptografarSenha(string senha)
    {
        var chave = ChaveCriptografia;
        var iv = RandomNumberGenerator.GetBytes(16);

        using var aes = Aes.Create();
        aes.Key = chave;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor();
        var senhaBytes = Encoding.UTF8.GetBytes(senha);
        var senhaCriptografada = encryptor.TransformFinalBlock(senhaBytes, 0, senhaBytes.Length);

        var resultado = new byte[iv.Length + senhaCriptografada.Length];
        Buffer.BlockCopy(iv, 0, resultado, 0, iv.Length);
        Buffer.BlockCopy(senhaCriptografada, 0, resultado, iv.Length, senhaCriptografada.Length);

        return Convert.ToBase64String(resultado);
    }

    public string DescriptografarSenha(string senhaCriptografada)
    {
        var chave = ChaveCriptografia;
        var dados = Convert.FromBase64String(senhaCriptografada);

        var iv = new byte[16];
        var cipherBytes = new byte[dados.Length - 16];
        Buffer.BlockCopy(dados, 0, iv, 0, 16);
        Buffer.BlockCopy(dados, 16, cipherBytes, 0, cipherBytes.Length);

        using var aes = Aes.Create();
        aes.Key = chave;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var senhaBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(senhaBytes);
    }

    public (bool Valido, string? Mensagem) ValidarCertificado(byte[] bytes, string senha, string cnpjEmpresa)
    {
        X509Certificate2 certificado;
        try
        {
            certificado = new X509Certificate2(bytes, senha, ResolverFlags());
        }
        catch
        {
            return (false, "Senha do certificado incorreta ou arquivo invalido.");
        }

        if (certificado.NotAfter.ToUniversalTime() < DateTime.UtcNow)
        {
            return (false, $"O certificado digital esta vencido desde {certificado.NotAfter:dd/MM/yyyy}.");
        }

        if (!certificado.HasPrivateKey)
        {
            return (false, "O certificado digital nao possui chave privada.");
        }

        var cnpjNoCertificado = ExtrairCnpjDoCertificado(certificado);

        if (!string.IsNullOrWhiteSpace(cnpjNoCertificado)
            && !string.IsNullOrWhiteSpace(cnpjEmpresa)
            && cnpjNoCertificado != new string(cnpjEmpresa.Where(char.IsDigit).ToArray()))
        {
            return (false,
                $"O CNPJ do certificado ({cnpjNoCertificado}) e diferente do CNPJ da empresa ({cnpjEmpresa}).");
        }

        return (true, null);
    }

    // No Windows, EphemeralKeySet impede o SSPI de acessar a chave privada no handshake mTLS.
    // PersistKeySet + MachineKeySet + Exportable garantem que o SChannel consiga usar a chave.
    // No Linux, EphemeralKeySet é suficiente e evita escrita em disco.
    private static X509KeyStorageFlags ResolverFlags()
    {
        if (OperatingSystem.IsWindows())
        {
            return X509KeyStorageFlags.Exportable
                 | X509KeyStorageFlags.MachineKeySet
                 | X509KeyStorageFlags.PersistKeySet;
        }

        return X509KeyStorageFlags.Exportable
             | X509KeyStorageFlags.EphemeralKeySet;
    }

    private static string? ExtrairCnpjDoCertificado(X509Certificate2 certificado)
    {
        var subject = certificado.Subject;

        var partes = subject.Split(',', StringSplitOptions.TrimEntries);
        foreach (var parte in partes)
        {
            if (!parte.StartsWith("CN=", StringComparison.OrdinalIgnoreCase)) continue;

            var cn = parte[3..];
            var digitos = new string(cn.Where(char.IsDigit).ToArray());
            if (digitos.Length == 14) return digitos;
            if (digitos.Length > 14)
            {
                var cnpj = digitos[^14..];
                if (cnpj.All(char.IsDigit)) return cnpj;
            }
        }

        return null;
    }
}
