namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

public sealed record RecebimentoResponse(
    ulong Id,
    ulong EmpresaId,
    ulong UsuarioEnvioId,
    string CodigoRecebimento,
    int Status,
    string StatusDescricao,
    string Origem,
    string? Observacao,
    DateTime DataEnvio,
    DateTime DataRecebimento,
    DateTime DataAtualizacao,
    IReadOnlyCollection<RecebimentoHistoricoResponse> Historicos);
