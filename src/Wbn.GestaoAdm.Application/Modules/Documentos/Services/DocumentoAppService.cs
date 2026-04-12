using Wbn.GestaoAdm.Application.Modules.Documentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Documentos.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Documentos.Services;

public sealed class DocumentoAppService(
    IArquivoRepository arquivoRepository,
    IRecebimentoRepository recebimentoRepository,
    IRecebimentoHistoricoRepository recebimentoHistoricoRepository,
    ITipoDocumentoRepository tipoDocumentoRepository,
    IUsuarioRepository usuarioRepository) : IDocumentoAppService
{
    public async Task<CriarDocumentoResponse> CreateAsync(CriarDocumentoRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureRecebimentoExistsAsync(request.RecebimentoId, cancellationToken);

        var tipoDocumento = await EnsureTipoDocumentoAtivoAsync(request.TipoDocumentoId, cancellationToken);

        if (request.UsuarioAcaoId.HasValue)
        {
            await EnsureUsuarioExistsAsync(request.UsuarioAcaoId.Value, cancellationToken);
        }

        var arquivo = new Arquivo(
            request.RecebimentoId,
            request.TipoDocumentoId,
            request.NomeOriginal,
            request.NomeArquivo,
            request.CaminhoArquivo,
            request.Extensao,
            request.MimeType,
            request.TamanhoBytes,
            request.OrdemExibicao,
            ativo: true);

        if (!arquivo.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", arquivo.Errors));
        }

        await arquivoRepository.Create(arquivo, cancellationToken);

        var historico = new RecebimentoHistorico(
            request.RecebimentoId,
            request.UsuarioAcaoId,
            AcoesHistorico.ArquivoAdicionado,
            $"Documento {arquivo.NomeOriginal} do tipo {tipoDocumento.Nome} adicionado ao recebimento.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var created = await arquivoRepository.GetDetailsByIdAsync(arquivo.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o documento apos o cadastro.");

        return MapToCreateResponse(created);
    }

    public async Task<DocumentoResponse?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var arquivo = await arquivoRepository.GetDetailsByIdAsync(id, cancellationToken);
        return arquivo is null ? null : MapToResponse(arquivo);
    }

    public async Task<IReadOnlyCollection<DocumentoResponse>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default)
    {
        await EnsureRecebimentoExistsAsync(recebimentoId, cancellationToken);

        var arquivos = await arquivoRepository.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);

        return arquivos
            .Select(MapToResponse)
            .ToArray();
    }

    public async Task<DocumentoResponse?> InativarAsync(
        ulong id,
        InativarDocumentoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.UsuarioAcaoId.HasValue)
        {
            await EnsureUsuarioExistsAsync(request.UsuarioAcaoId.Value, cancellationToken);
        }

        var arquivo = await arquivoRepository.Get(id, cancellationToken);
        if (arquivo is null)
        {
            return null;
        }

        if (!arquivo.Ativo)
        {
            var arquivoInativo = await arquivoRepository.GetDetailsByIdAsync(id, cancellationToken)
                ?? throw new RegraDeNegocioException("Nao foi possivel carregar o documento inativo.");

            return MapToResponse(arquivoInativo);
        }

        arquivo.Inativar();

        if (!arquivo.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", arquivo.Errors));
        }

        await arquivoRepository.Update(arquivo, cancellationToken);

        var historico = new RecebimentoHistorico(
            arquivo.RecebimentoId,
            request.UsuarioAcaoId,
            AcoesHistorico.ArquivoInativado,
            $"Documento {arquivo.NomeOriginal} inativado.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var updated = await arquivoRepository.GetDetailsByIdAsync(id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o documento apos a inativacao.");

        return MapToResponse(updated);
    }

    public async Task<IReadOnlyCollection<TipoDocumentoResponse>> GetTiposDocumentoAtivosAsync(CancellationToken cancellationToken = default)
    {
        var tiposDocumento = await tipoDocumentoRepository.GetAllAtivosAsync(cancellationToken);

        return tiposDocumento
            .OrderBy(tipoDocumento => tipoDocumento.Nome)
            .Select(MapTipoDocumentoToResponse)
            .ToArray();
    }

    private async Task EnsureRecebimentoExistsAsync(ulong recebimentoId, CancellationToken cancellationToken)
    {
        if (!await recebimentoRepository.RecordExists(recebimentoId, cancellationToken))
        {
            throw new RegraDeNegocioException("O recebimento informado nao existe.");
        }
    }

    private async Task EnsureUsuarioExistsAsync(ulong usuarioId, CancellationToken cancellationToken)
    {
        if (!await usuarioRepository.RecordExists(usuarioId, cancellationToken))
        {
            throw new RegraDeNegocioException("O usuario informado nao existe.");
        }
    }

    private async Task<TipoDocumento> EnsureTipoDocumentoAtivoAsync(ulong tipoDocumentoId, CancellationToken cancellationToken)
    {
        var tipoDocumento = await tipoDocumentoRepository.Get(tipoDocumentoId, cancellationToken);

        if (tipoDocumento is null)
        {
            throw new RegraDeNegocioException("O tipo de documento informado nao existe.");
        }

        if (!tipoDocumento.Ativo)
        {
            throw new RegraDeNegocioException("O tipo de documento informado esta inativo.");
        }

        return tipoDocumento;
    }

    private static CriarDocumentoResponse MapToCreateResponse(Arquivo arquivo)
    {
        return new CriarDocumentoResponse(
            arquivo.Id,
            arquivo.RecebimentoId,
            arquivo.TipoDocumentoId,
            arquivo.TipoDocumento.Nome,
            arquivo.NomeOriginal,
            arquivo.NomeArquivo,
            arquivo.CaminhoArquivo,
            arquivo.Extensao,
            arquivo.MimeType,
            arquivo.TamanhoBytes,
            arquivo.OrdemExibicao,
            arquivo.Ativo,
            arquivo.DataUpload);
    }

    private static DocumentoResponse MapToResponse(Arquivo arquivo)
    {
        return new DocumentoResponse(
            arquivo.Id,
            arquivo.RecebimentoId,
            arquivo.TipoDocumentoId,
            arquivo.TipoDocumento.Nome,
            arquivo.NomeOriginal,
            arquivo.NomeArquivo,
            arquivo.CaminhoArquivo,
            arquivo.Extensao,
            arquivo.MimeType,
            arquivo.TamanhoBytes,
            arquivo.OrdemExibicao,
            arquivo.Ativo,
            arquivo.DataUpload);
    }

    private static TipoDocumentoResponse MapTipoDocumentoToResponse(TipoDocumento tipoDocumento)
    {
        return new TipoDocumentoResponse(
            tipoDocumento.Id,
            tipoDocumento.Nome,
            tipoDocumento.Descricao,
            tipoDocumento.Ativo,
            tipoDocumento.DataCadastro);
    }
}
