namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileRecebimentoDetalheResponse(
    ulong Id,
    string CodigoRecebimento,
    ulong EmpresaId,
    string EmpresaNome,
    int Status,
    string StatusDescricao,
    string? Observacao,
    DateTime DataEnvio,
    IReadOnlyCollection<MobileNotaFiscalResponse> NotasFiscais,
    IReadOnlyCollection<MobileDocumentoResponse> Documentos,
    IReadOnlyCollection<MobileHistoricoResponse> Historico);

public sealed record MobileNotaFiscalResponse(
    ulong Id,
    string? NumeroNota,
    string? Serie,
    string? ChaveAcesso,
    decimal ValorTotal,
    DateTime? DataEmissao,
    DateTime? DataEntrada,
    string? CpfCnpjEmitente,
    string? NomeEmitente,
    string? Observacao);

public sealed record MobileHistoricoResponse(
    string Acao,
    string Descricao,
    DateTime DataCadastro);
