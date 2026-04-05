namespace Wbn.GestaoAdm.Application.Modules.Empresas.Dtos;

public sealed record EmpresaLookupResponse(
    ulong Id,
    string NomeFantasia,
    string RazaoSocial,
    string Cnpj,
    bool Ativo);
