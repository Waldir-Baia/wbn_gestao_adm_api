namespace Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;

public sealed record ResolverDivergenciaRequest(
    ulong UsuarioResolucaoId,
    string ObservacaoResolucao);
