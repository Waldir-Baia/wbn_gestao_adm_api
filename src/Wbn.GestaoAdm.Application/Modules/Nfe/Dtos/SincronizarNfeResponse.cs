namespace Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;

public sealed record SincronizarNfeResponse(
    bool Sucesso,
    string Mensagem,
    int QuantidadeDocumentosProcessados,
    string UltimoNsu,
    string MaxNsu,
    DateTime ProximaConsultaPermitidaEm);
