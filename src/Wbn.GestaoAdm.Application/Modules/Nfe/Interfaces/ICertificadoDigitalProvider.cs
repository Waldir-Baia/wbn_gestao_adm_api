using System.Security.Cryptography.X509Certificates;

namespace Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;

public interface ICertificadoDigitalProvider
{
    X509Certificate2 CarregarCertificado(byte[] bytes, string senhaDecriptografada);
    string CriptografarSenha(string senha);
    string DescriptografarSenha(string senhaCriptografada);
    (bool Valido, string? Mensagem) ValidarCertificado(byte[] bytes, string senha, string cnpjEmpresa);
}
