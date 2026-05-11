using Wbn.GestaoAdm.Domain.Modules.Nfe.Enums;

namespace Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;

public sealed record ManifestarNfeRequest(
    ulong EmpresaId,
    TipoManifestacaoNfeEnum TipoManifestacao,
    string? Justificativa = null);
