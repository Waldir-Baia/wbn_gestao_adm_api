namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

public sealed record CreateRecebimentoRequest(
    ulong EmpresaId,
    ulong UsuarioEnvioId,
    string CodigoRecebimento,
    string Origem,
    string? Observacao,
    DateTime? DataEnvio);
