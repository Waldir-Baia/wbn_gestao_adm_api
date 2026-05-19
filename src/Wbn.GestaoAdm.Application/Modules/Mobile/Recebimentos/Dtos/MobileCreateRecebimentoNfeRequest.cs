namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileCreateRecebimentoNfeRequest(
    ulong EmpresaId,
    ulong UsuarioId,
    string ChaveAcesso,
    string? Observacao);
