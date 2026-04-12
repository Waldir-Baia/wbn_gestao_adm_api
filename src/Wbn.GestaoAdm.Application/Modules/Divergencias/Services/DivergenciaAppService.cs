using Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Enums;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Divergencias.Services;

public sealed class DivergenciaAppService(
    IRecebimentoDivergenciaRepository recebimentoDivergenciaRepository,
    IRecebimentoRepository recebimentoRepository,
    IRecebimentoHistoricoRepository recebimentoHistoricoRepository,
    IUsuarioRepository usuarioRepository) : IDivergenciaAppService
{
    public async Task<DivergenciaResponse> CreateAsync(CreateDivergenciaRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUsuarioExistsAsync(request.UsuarioId, cancellationToken);

        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(request.RecebimentoId, cancellationToken)
            ?? throw new RegraDeNegocioException("O recebimento informado nao existe.");

        var divergencia = new RecebimentoDivergencia(
            request.RecebimentoId,
            request.UsuarioId,
            request.TipoDivergencia,
            request.Descricao);

        if (!divergencia.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", divergencia.Errors));
        }

        await recebimentoDivergenciaRepository.Create(divergencia, cancellationToken);

        if (recebimento.StatusRecebimento != RecebimentoStatusEnum.ComDivergencia)
        {
            recebimento.AtualizarStatus(RecebimentoStatusEnum.ComDivergencia, request.Descricao);

            if (!recebimento.Validate())
            {
                throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
            }

            await recebimentoRepository.Update(recebimento, cancellationToken);
        }

        var historico = new RecebimentoHistorico(
            recebimento.Id,
            request.UsuarioId,
            AcoesHistorico.DivergenciaCriada,
            $"Divergência criada com status {(int)divergencia.StatusDivergencia} - {divergencia.StatusDivergencia}. Tipo: {divergencia.TipoDivergencia}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var created = await recebimentoDivergenciaRepository.GetDetailsByIdAsync(divergencia.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar a divergência apos o cadastro.");

        return MapToResponse(created);
    }

    public async Task<DivergenciaResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var divergencia = await recebimentoDivergenciaRepository.GetDetailsByIdAsync(id, cancellationToken);
        return divergencia is null ? null : MapToResponse(divergencia);
    }

    public async Task<DivergenciaResponse?> UpdateStatusAsync(
        ulong id,
        UpdateDivergenciaStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureUsuarioExistsAsync(request.UsuarioAcaoId, cancellationToken);

        var divergencia = await recebimentoDivergenciaRepository.Get(id, cancellationToken);
        if (divergencia is null)
        {
            return null;
        }

        var statusAnterior = divergencia.StatusDivergencia;
        divergencia.AlterarStatus((DivergenciaStatusEnum)request.Status);

        if (!divergencia.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", divergencia.Errors));
        }

        await recebimentoDivergenciaRepository.Update(divergencia, cancellationToken);

        var historico = new RecebimentoHistorico(
            divergencia.RecebimentoId,
            request.UsuarioAcaoId,
            AcoesHistorico.StatusDivergenciaAlterado,
            $"Status da divergência alterado de {(int)statusAnterior} - {statusAnterior} para {(int)divergencia.StatusDivergencia} - {divergencia.StatusDivergencia}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var updated = await recebimentoDivergenciaRepository.GetDetailsByIdAsync(id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar a divergência apos a atualizacao.");

        return MapToResponse(updated);
    }

    public async Task<DivergenciaResponse?> ResolverAsync(
        ulong id,
        ResolverDivergenciaRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureUsuarioExistsAsync(request.UsuarioResolucaoId, cancellationToken);

        var divergencia = await recebimentoDivergenciaRepository.Get(id, cancellationToken);
        if (divergencia is null)
        {
            return null;
        }

        divergencia.Resolver(request.UsuarioResolucaoId, request.ObservacaoResolucao);

        if (!divergencia.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", divergencia.Errors));
        }

        await recebimentoDivergenciaRepository.Update(divergencia, cancellationToken);

        var historico = new RecebimentoHistorico(
            divergencia.RecebimentoId,
            request.UsuarioResolucaoId,
            AcoesHistorico.DivergenciaResolvida,
            $"Divergência resolvida com status {(int)divergencia.StatusDivergencia} - {divergencia.StatusDivergencia}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var resolved = await recebimentoDivergenciaRepository.GetDetailsByIdAsync(id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar a divergência apos a resolucao.");

        return MapToResponse(resolved);
    }

    private async Task EnsureUsuarioExistsAsync(ulong usuarioId, CancellationToken cancellationToken)
    {
        if (!await usuarioRepository.RecordExists(usuarioId, cancellationToken))
        {
            throw new RegraDeNegocioException("O usuario informado nao existe.");
        }
    }

    private static DivergenciaResponse MapToResponse(RecebimentoDivergencia divergencia)
    {
        return new DivergenciaResponse(
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
            divergencia.ObservacaoResolucao);
    }
}
