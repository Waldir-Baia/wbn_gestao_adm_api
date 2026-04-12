using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;
using Wbn.GestaoAdm.Application.Modules.Cfg.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Enums;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Conferencia.Services;

public sealed class ConferenciaAppService(
    ICfgConsultaAppService cfgConsultaAppService,
    IRecebimentoRepository recebimentoRepository,
    IRecebimentoConferenciaRepository recebimentoConferenciaRepository,
    IRecebimentoDivergenciaRepository recebimentoDivergenciaRepository,
    IRecebimentoHistoricoRepository recebimentoHistoricoRepository,
    IUsuarioRepository usuarioRepository) : IConferenciaAppService
{
    public async Task<CfgResultDto> GetFilaAsync(ConferenciaFilaRequest request, CancellationToken cancellationToken = default)
    {
        var cfgRequest = new CfgRequestDataDto(
            request.Filter,
            request.FilterCode,
            request.Page,
            request.DataCountByPage,
            request.OrderByField,
            request.OrderByType);

        return await cfgConsultaAppService.ProcessQueryAsync("conferenciaFila", cfgRequest, cancellationToken);
    }

    public async Task<ConferenciaDetalheResponse?> GetDetalheAsync(ulong recebimentoId, CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken);
        if (recebimento is null)
        {
            return null;
        }

        var conferencias = await recebimentoConferenciaRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);
        var divergencias = await recebimentoDivergenciaRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);
        return MapToDetalheResponse(recebimento, conferencias, divergencias);
    }

    public async Task<ConferenciaDetalheResponse?> IniciarAsync(
        ulong recebimentoId,
        IniciarConferenciaRequest request,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken);
        if (recebimento is null)
        {
            return null;
        }

        await EnsureUsuarioExistsAsync(request.UsuarioAcaoId, cancellationToken);

        recebimento.IniciarConferencia(request.Observacao);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Update(recebimento, cancellationToken);

        var historico = new RecebimentoHistorico(
            recebimento.Id,
            request.UsuarioAcaoId,
            AcoesHistorico.ConferenciaIniciada,
            $"Conferência iniciada. Status alterado para {(int)RecebimentoStatusEnum.EmConferencia} - {RecebimentoStatusEnum.EmConferencia}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var atualizado = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos iniciar a conferência.");
        var conferencias = await recebimentoConferenciaRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);
        var divergencias = await recebimentoDivergenciaRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);

        return MapToDetalheResponse(atualizado, conferencias, divergencias);
    }

    public async Task<ConferenciaDetalheResponse?> FinalizarAsync(
        ulong recebimentoId,
        FinalizarConferenciaRequest request,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken);
        if (recebimento is null)
        {
            return null;
        }

        await EnsureUsuarioExistsAsync(request.UsuarioConferenciaId, cancellationToken);

        var conferencia = new RecebimentoConferencia(
            recebimentoId,
            request.UsuarioConferenciaId,
            request.StatusConferencia,
            request.NotaEncontrada,
            request.BoletoEncontrado,
            request.ValorConfere,
            request.DataVencimentoConfere,
            request.DocumentoLegivel,
            request.Observacao);

        if (!conferencia.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", conferencia.Errors));
        }

        var novoStatus = conferencia.PossuiDivergencia()
            ? RecebimentoStatusEnum.ComDivergencia
            : RecebimentoStatusEnum.Conferido;

        recebimento.FinalizarConferencia(novoStatus, request.Observacao);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Update(recebimento, cancellationToken);
        await recebimentoConferenciaRepository.Create(conferencia, cancellationToken);

        var historico = new RecebimentoHistorico(
            recebimento.Id,
            request.UsuarioConferenciaId,
            AcoesHistorico.ConferenciaRealizada,
            $"Conferência finalizada com status {(int)novoStatus} - {novoStatus}. Resultado da conferência: {conferencia.StatusConferencia}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var atualizado = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos finalizar a conferência.");
        var conferencias = await recebimentoConferenciaRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);
        var divergencias = await recebimentoDivergenciaRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);

        return MapToDetalheResponse(atualizado, conferencias, divergencias);
    }

    private async Task EnsureUsuarioExistsAsync(ulong usuarioId, CancellationToken cancellationToken)
    {
        if (!await usuarioRepository.RecordExists(usuarioId, cancellationToken))
        {
            throw new RegraDeNegocioException("O usuario informado nao existe.");
        }
    }

    private static ConferenciaDetalheResponse MapToDetalheResponse(
        Recebimento recebimento,
        IReadOnlyCollection<RecebimentoConferencia> conferencias,
        IReadOnlyCollection<RecebimentoDivergencia> divergencias)
    {
        var recebimentoResponse = new RecebimentoResponse(
            recebimento.Id,
            recebimento.EmpresaId,
            recebimento.UsuarioEnvioId,
            recebimento.CodigoRecebimento,
            (int)recebimento.StatusRecebimento,
            recebimento.StatusRecebimento.ToString(),
            recebimento.Origem,
            recebimento.Observacao,
            recebimento.DataEnvio,
            recebimento.DataRecebimento,
            recebimento.DataAtualizacao,
            recebimento.Historicos
                .OrderBy(historico => historico.DataCadastro)
                .Select(historico => new RecebimentoHistoricoResponse(
                    historico.Id,
                    historico.RecebimentoId,
                    historico.UsuarioId,
                    historico.Acao,
                    historico.Descricao,
                    historico.DataCadastro))
                .ToArray());

        return new ConferenciaDetalheResponse(
            recebimentoResponse,
            conferencias.Select(conferencia => new ConferenciaRealizadaResponse(
                    conferencia.Id,
                    conferencia.RecebimentoId,
                    conferencia.UsuarioConferenciaId,
                    conferencia.StatusConferencia,
                    conferencia.NotaEncontrada,
                    conferencia.BoletoEncontrado,
                    conferencia.ValorConfere,
                    conferencia.DataVencimentoConfere,
                    conferencia.DocumentoConfere,
                    conferencia.Observacao,
                    conferencia.DataConferencia))
                .ToArray(),
            recebimento.Arquivos.Select(arquivo => new ConferenciaArquivoResponse(
                    arquivo.Id,
                    arquivo.TipoDocumentoId,
                    arquivo.NomeOriginal,
                    arquivo.NomeArquivo,
                    arquivo.CaminhoArquivo,
                    arquivo.Extensao,
                    arquivo.MimeType,
                    arquivo.TamanhoBytes,
                    arquivo.OrdemExibicao,
                    arquivo.Ativo,
                    arquivo.DataUpload))
                .ToArray(),
            recebimento.NotasFiscais.Select(nota => new ConferenciaNotaFiscalResponse(
                    nota.Id,
                    nota.ArquivoId,
                    nota.NumeroNota,
                    nota.Serie,
                    nota.ChaveAcesso,
                    nota.ValorTotal,
                    nota.DataEmissao,
                    nota.DataEntrada,
                    nota.CpfCnpjEmitente,
                    nota.NomeEmitente,
                    nota.Observacao))
                .ToArray(),
            recebimento.Boletos.Select(boleto => new ConferenciaBoletoResponse(
                    boleto.Id,
                    boleto.ArquivoId,
                    boleto.CodigoBarras,
                    boleto.LinhaDigitavel,
                    boleto.ValorBoleto,
                    boleto.DataVencimento,
                    boleto.DataEmissao,
                    boleto.Beneficiario,
                    boleto.DocumentoBeneficiario,
                    boleto.Observacao))
                .ToArray(),
            divergencias.Select(divergencia => new DivergenciaResponse(
                    divergencia.Id,
                    divergencia.RecebimentoId,
                    divergencia.UsuarioId,
                    divergencia.TipoDivergencia,
                    divergencia.Descricao,
                    (int)divergencia.StatusDivergencia,
                    divergencia.StatusDivergencia.ToString(),
                    divergencia.Resolvida,
                    divergencia.DataCadastro,
                    divergencia.DataAtualizacao,
                    divergencia.DataResolucao,
                    divergencia.UsuarioResolucaoId,
                    divergencia.ObservacaoResolucao))
                .ToArray());
    }
}
