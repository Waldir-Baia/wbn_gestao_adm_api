namespace Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Dtos;

public sealed record MobileLoginResult(
    ulong Id,
    ulong PerfilId,
    string Nome,
    string Email,
    string? Telefone,
    IReadOnlyCollection<MobileEmpresaVinculadaResult> Empresas);

public sealed record MobileEmpresaVinculadaResult(ulong Id, string NomeFantasia);
