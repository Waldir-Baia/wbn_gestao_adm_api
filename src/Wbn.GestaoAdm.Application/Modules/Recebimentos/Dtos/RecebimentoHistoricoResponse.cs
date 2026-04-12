namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

public sealed record RecebimentoHistoricoResponse(
    ulong Id,
    ulong RecebimentoId,
    ulong? UsuarioId,
    string Acao,
    string Descricao,
    DateTime DataCadastro);
