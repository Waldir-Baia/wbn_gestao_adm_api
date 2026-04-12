namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;

public sealed record FinalizarConferenciaRequest(
    ulong UsuarioConferenciaId,
    bool NotaEncontrada,
    bool BoletoEncontrado,
    bool ValorConfere,
    bool DataVencimentoConfere,
    bool DocumentoLegivel,
    string? Observacao,
    string StatusConferencia);
