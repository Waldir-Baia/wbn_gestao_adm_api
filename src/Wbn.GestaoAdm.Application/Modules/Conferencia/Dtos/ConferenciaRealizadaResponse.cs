namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record ConferenciaRealizadaResponse(
    ulong Id,
    ulong RecebimentoId,
    ulong UsuarioConferenciaId,
    string StatusConferencia,
    bool NotaEncontrada,
    bool BoletoEncontrado,
    bool ValorConfere,
    bool DataVencimentoConfere,
    bool DocumentoLegivel,
    string? Observacao,
    DateTime DataConferencia);
