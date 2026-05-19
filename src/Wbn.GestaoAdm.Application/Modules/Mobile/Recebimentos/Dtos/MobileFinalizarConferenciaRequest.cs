namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileFinalizarConferenciaRequest(
    bool PossuiDivergencia,
    string? Observacao);
