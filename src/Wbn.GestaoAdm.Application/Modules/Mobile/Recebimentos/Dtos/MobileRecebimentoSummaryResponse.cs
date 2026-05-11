namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;

public sealed record MobileRecebimentoSummaryResponse(
    ulong Id,
    string CodigoRecebimento,
    ulong EmpresaId,
    string EmpresaNome,
    int Status,
    string StatusDescricao,
    DateTime DataEnvio,
    int QuantidadeArquivos);
