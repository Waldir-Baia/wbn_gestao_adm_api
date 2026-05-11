namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileCreateRecebimentoRequest(
    ulong EmpresaId,
    ulong UsuarioId,
    string? Observacao);
