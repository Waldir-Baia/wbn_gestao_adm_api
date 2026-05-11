using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Services;

public sealed class MobileRecebimentoAppService(
    IRecebimentoRepository recebimentoRepository,
    IRecebimentoHistoricoRepository recebimentoHistoricoRepository,
    IArquivoRepository arquivoRepository,
    IUsuarioRepository usuarioRepository,
    ITipoDocumentoRepository tipoDocumentoRepository) : IMobileRecebimentoAppService
{
    public async Task<IReadOnlyCollection<MobileRecebimentoSummaryResponse>> GetMeusEnviosAsync(
        ulong usuarioId,
        CancellationToken cancellationToken = default)
    {
        var recebimentos = await recebimentoRepository.GetByUsuarioEnvioIdAsync(usuarioId, cancellationToken);

        return recebimentos
            .Select(r => new MobileRecebimentoSummaryResponse(
                r.Id,
                r.CodigoRecebimento,
                r.EmpresaId,
                r.Empresa.NomeFantasia,
                (int)r.StatusRecebimento,
                r.StatusRecebimento.ToString(),
                r.DataEnvio,
                r.Arquivos.Count(a => a.Ativo)))
            .ToArray();
    }

    public async Task<MobileRecebimentoDetalheResponse?> GetDetalheAsync(
        ulong id,
        ulong usuarioId,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(id, cancellationToken);

        if (recebimento is null)
        {
            return null;
        }

        if (recebimento.UsuarioEnvioId != usuarioId)
        {
            throw new RegraDeNegocioException("Acesso negado ao recebimento informado.");
        }

        return MapToDetalhe(recebimento);
    }

    public async Task<MobileRecebimentoDetalheResponse> CreateAsync(
        MobileCreateRecebimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureUsuarioTemAcessoEmpresaAsync(request.UsuarioId, request.EmpresaId, cancellationToken);

        var codigo = GerarCodigoRecebimento();
        await EnsureCodigoUnicoAsync(codigo, cancellationToken);

        var recebimento = new Recebimento(
            request.EmpresaId,
            request.UsuarioId,
            codigo,
            OrigensRecebimento.Mobile,
            request.Observacao);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Create(recebimento, cancellationToken);

        var historico = new RecebimentoHistorico(
            recebimento.Id,
            request.UsuarioId,
            AcoesHistorico.RecebimentoCriado,
            $"Recebimento {recebimento.CodigoRecebimento} criado pelo aplicativo mobile com status {(int)recebimento.StatusRecebimento} - {recebimento.StatusRecebimento}.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var criado = await recebimentoRepository.GetDetailsByIdAsync(recebimento.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos o cadastro.");

        return MapToDetalhe(criado);
    }

    public async Task<MobileDocumentoResponse> EnviarDocumentoAsync(
        MobileEnviarDocumentoRequest request,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(request.RecebimentoId, cancellationToken)
            ?? throw new RegraDeNegocioException("Recebimento nao encontrado.");

        if (recebimento.UsuarioEnvioId != request.UsuarioId)
        {
            throw new RegraDeNegocioException("Acesso negado ao recebimento informado.");
        }

        var tipoDocumento = await tipoDocumentoRepository.Get(request.TipoDocumentoId, cancellationToken)
            ?? throw new RegraDeNegocioException("Tipo de documento nao encontrado.");

        if (!tipoDocumento.Ativo)
        {
            throw new RegraDeNegocioException("O tipo de documento informado esta inativo.");
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
            request.UsuarioId,
            AcoesHistorico.ArquivoAdicionado,
            $"Documento {arquivo.NomeOriginal} do tipo {tipoDocumento.Nome} adicionado via aplicativo mobile.");

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);

        var salvo = await arquivoRepository.GetDetailsByIdAsync(arquivo.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o documento apos o envio.");

        return new MobileDocumentoResponse(
            salvo.Id,
            salvo.TipoDocumentoId,
            salvo.TipoDocumento.Nome,
            salvo.NomeOriginal,
            salvo.Extensao,
            salvo.MimeType,
            salvo.TamanhoBytes,
            salvo.OrdemExibicao,
            salvo.DataUpload);
    }

    private async Task EnsureUsuarioTemAcessoEmpresaAsync(
        ulong usuarioId,
        ulong empresaId,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.GetByIdForAuthenticationAsync(usuarioId, cancellationToken)
            ?? throw new RegraDeNegocioException("Usuario nao encontrado.");

        var temAcesso = usuario.UsuariosEmpresas.Any(v => v.EmpresaId == empresaId && v.Ativo);

        if (!temAcesso)
        {
            throw new RegraDeNegocioException("Usuario nao possui acesso a empresa informada.");
        }
    }

    private async Task EnsureCodigoUnicoAsync(string codigo, CancellationToken cancellationToken)
    {
        var existente = await recebimentoRepository.GetByCodigoRecebimentoAsync(codigo, cancellationToken);

        if (existente is not null)
        {
            throw new RegraDeNegocioException("Erro ao gerar codigo do recebimento. Tente novamente.");
        }
    }

    private static string GerarCodigoRecebimento()
    {
        var sufixo = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return $"MOB-{DateTime.UtcNow:yyyyMMdd}-{sufixo}";
    }

    private static MobileRecebimentoDetalheResponse MapToDetalhe(Recebimento recebimento)
    {
        var documentos = recebimento.Arquivos
            .Where(a => a.Ativo)
            .OrderBy(a => a.OrdemExibicao)
            .ThenBy(a => a.DataUpload)
            .Select(a => new MobileDocumentoResponse(
                a.Id,
                a.TipoDocumentoId,
                a.TipoDocumento?.Nome ?? string.Empty,
                a.NomeOriginal,
                a.Extensao,
                a.MimeType,
                a.TamanhoBytes,
                a.OrdemExibicao,
                a.DataUpload))
            .ToArray();

        var historico = recebimento.Historicos
            .OrderBy(h => h.DataCadastro)
            .Select(h => new MobileHistoricoResponse(h.Acao, h.Descricao, h.DataCadastro))
            .ToArray();

        return new MobileRecebimentoDetalheResponse(
            recebimento.Id,
            recebimento.CodigoRecebimento,
            recebimento.EmpresaId,
            recebimento.Empresa?.NomeFantasia ?? string.Empty,
            (int)recebimento.StatusRecebimento,
            recebimento.StatusRecebimento.ToString(),
            recebimento.Observacao,
            recebimento.DataEnvio,
            documentos,
            historico);
    }
}
