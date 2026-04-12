using Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;

namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record ConferenciaDetalheResponse(
    RecebimentoResponse Recebimento,
    IReadOnlyCollection<ConferenciaRealizadaResponse> ConferenciasRealizadas,
    IReadOnlyCollection<ConferenciaArquivoResponse> Arquivos,
    IReadOnlyCollection<ConferenciaNotaFiscalResponse> NotasFiscais,
    IReadOnlyCollection<ConferenciaBoletoResponse> Boletos,
    IReadOnlyCollection<DivergenciaResponse> Divergencias);
