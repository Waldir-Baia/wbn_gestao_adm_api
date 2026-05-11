namespace Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;

public sealed record AtualizarCertificadoDigitalEmpresaRequest(
    ulong EmpresaId,
    string CertificadoBase64,
    string SenhaCertificado,
    int CodigoUf,
    bool CertificadoDigitalAtivo = true);
