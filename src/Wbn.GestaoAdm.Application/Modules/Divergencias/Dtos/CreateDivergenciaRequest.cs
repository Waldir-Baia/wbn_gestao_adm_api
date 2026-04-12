namespace Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;

public sealed record CreateDivergenciaRequest(
    ulong RecebimentoId,
    ulong UsuarioId,
    string TipoDivergencia,
    string Descricao);
