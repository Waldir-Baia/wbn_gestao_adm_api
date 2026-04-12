using Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Enums;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Recebimentos.Services;

public sealed class RecebimentoAppService(
    IRecebimentoRepository recebimentoRepository,
    IRecebimentoHistoricoRepository recebimentoHistoricoRepository,
    IEmpresaRepository empresaRepository,
    IUsuarioRepository usuarioRepository) : IRecebimentoAppService
{
    public async Task<RecebimentoResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(id, cancellationToken);
        return recebimento is null ? null : MapToResponse(recebimento);
    }

    public async Task<RecebimentoResponse> CreateAsync(
        CreateRecebimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureEmpresaExistsAsync(request.EmpresaId, cancellationToken);
        await EnsureUsuarioExistsAsync(request.UsuarioEnvioId, cancellationToken);
        await EnsureCodigoRecebimentoIsUniqueAsync(request.CodigoRecebimento, cancellationToken);

        var recebimento = new Recebimento(
            request.EmpresaId,
            request.UsuarioEnvioId,
            request.CodigoRecebimento,
            request.Origem,
            request.Observacao,
            request.DataEnvio);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Create(recebimento, cancellationToken);

        var historico = new RecebimentoHistorico(
            recebimento.Id,
            request.UsuarioEnvioId,
            AcoesHistorico.RecebimentoCriado,
            $"Recebimento {recebimento.CodigoRecebimento} criado com status {(int)recebimento.StatusRecebimento} - {recebimento.StatusRecebimento}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var recebimentoCriado = await recebimentoRepository.GetDetailsByIdAsync(recebimento.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos o cadastro.");

        return MapToResponse(recebimentoCriado);
    }

    public async Task<RecebimentoResponse?> UpdateStatusAsync(
        ulong id,
        UpdateRecebimentoStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(id, cancellationToken);

        if (recebimento is null)
        {
            return null;
        }

        if (request.UsuarioAcaoId.HasValue)
        {
            await EnsureUsuarioExistsAsync(request.UsuarioAcaoId.Value, cancellationToken);
        }

        var statusAnterior = recebimento.StatusRecebimento;
        recebimento.AtualizarStatus((RecebimentoStatusEnum)request.Status, request.Observacao);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Update(recebimento, cancellationToken);

        var historico = new RecebimentoHistorico(
            recebimento.Id,
            request.UsuarioAcaoId,
            AcoesHistorico.StatusAlterado,
            $"Status alterado de {(int)statusAnterior} - {statusAnterior} para {(int)recebimento.StatusRecebimento} - {recebimento.StatusRecebimento}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var recebimentoAtualizado = await recebimentoRepository.GetDetailsByIdAsync(recebimento.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos a atualizacao.");

        return MapToResponse(recebimentoAtualizado);
    }

    private async Task EnsureEmpresaExistsAsync(ulong empresaId, CancellationToken cancellationToken)
    {
        if (!await empresaRepository.RecordExists(empresaId, cancellationToken))
        {
            throw new RegraDeNegocioException("A empresa informada nao existe.");
        }
    }

    private async Task EnsureUsuarioExistsAsync(ulong usuarioId, CancellationToken cancellationToken)
    {
        if (!await usuarioRepository.RecordExists(usuarioId, cancellationToken))
        {
            throw new RegraDeNegocioException("O usuario informado nao existe.");
        }
    }

    private async Task EnsureCodigoRecebimentoIsUniqueAsync(string codigoRecebimento, CancellationToken cancellationToken)
    {
        var recebimentoExistente = await recebimentoRepository.GetByCodigoRecebimentoAsync(codigoRecebimento, cancellationToken);

        if (recebimentoExistente is not null)
        {
            throw new RegraDeNegocioException("Ja existe um recebimento cadastrado com este codigo.");
        }
    }

    private static RecebimentoResponse MapToResponse(Recebimento recebimento)
    {
        return new RecebimentoResponse(
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
                .Select(MapHistoricoToResponse)
                .ToArray());
    }

    private static RecebimentoHistoricoResponse MapHistoricoToResponse(RecebimentoHistorico historico)
    {
        return new RecebimentoHistoricoResponse(
            historico.Id,
            historico.RecebimentoId,
            historico.UsuarioId,
            historico.Acao,
            historico.Descricao,
            historico.DataCadastro);
    }
}
