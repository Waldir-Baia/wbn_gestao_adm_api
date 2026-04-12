namespace Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;

public sealed record UpdateDivergenciaStatusRequest(
    int Status,
    ulong UsuarioAcaoId);
