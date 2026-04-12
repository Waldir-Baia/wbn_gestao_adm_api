namespace Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;

public sealed record DivergenciaResponse(
    ulong Id,
    ulong RecebimentoId,
    ulong UsuarioId,
    string TipoDivergencia,
    string Descricao,
    int Status,
    string StatusDescricao,
    bool Resolvida,
    DateTime DataCadastro,
    DateTime? DataAtualizacao,
    DateTime? DataResolucao,
    ulong? UsuarioResolucaoId,
    string? ObservacaoResolucao);
