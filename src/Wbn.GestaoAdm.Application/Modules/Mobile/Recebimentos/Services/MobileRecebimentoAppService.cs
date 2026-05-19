using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Nfe.Services;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Constants;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Enums;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Services;

public sealed class MobileRecebimentoAppService(
    IRecebimentoRepository recebimentoRepository,
    IRecebimentoHistoricoRepository recebimentoHistoricoRepository,
    IRecebimentoConferenciaRepository recebimentoConferenciaRepository,
    INotaFiscalRepository notaFiscalRepository,
    IArquivoRepository arquivoRepository,
    INfeDocumentoRepository nfeDocumentoRepository,
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

    public async Task<MobileRecebimentoDetalheResponse> CreateFromNfeAsync(
        MobileCreateRecebimentoNfeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ChaveAcesso);

        if (request.ChaveAcesso.Trim().Length != 44)
        {
            throw new RegraDeNegocioException("A chave de acesso deve conter 44 digitos.");
        }

        await EnsureUsuarioTemAcessoEmpresaAsync(request.UsuarioId, request.EmpresaId, cancellationToken);

        var chaveAcesso = request.ChaveAcesso.Trim();
        var notaExistente = await notaFiscalRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken);
        if (notaExistente?.Recebimento is not null)
        {
            if (notaExistente.Recebimento.UsuarioEnvioId != request.UsuarioId)
            {
                throw new RegraDeNegocioException("Esta nota fiscal ja esta vinculada a outro recebimento.");
            }

            var detalheExistente = await GetDetalheAsync(notaExistente.RecebimentoId, request.UsuarioId, cancellationToken);
            if (detalheExistente is not null)
            {
                return detalheExistente;
            }
        }

        var documentoNfe = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken)
            ?? throw new RegraDeNegocioException("Nota fiscal nao encontrada. Busque a NF-e antes de criar o recebimento.");

        if (documentoNfe.EmpresaId != request.EmpresaId)
        {
            throw new RegraDeNegocioException("A nota fiscal encontrada nao pertence a empresa informada.");
        }

        if (string.IsNullOrWhiteSpace(documentoNfe.XmlCompleto))
        {
            throw new RegraDeNegocioException("Nota fiscal ainda nao possui XML completo. Busque a NF-e antes de criar o recebimento.");
        }

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

        await CriarHistoricoAsync(
            recebimento.Id,
            request.UsuarioId,
            AcoesHistorico.RecebimentoCriado,
            $"Recebimento {recebimento.CodigoRecebimento} criado a partir da NF-e {chaveAcesso} com status {(int)recebimento.StatusRecebimento} - {recebimento.StatusRecebimento}.",
            cancellationToken);

        var dadosNfe = NfeXmlParser.ParsearNfeCompleta(documentoNfe.XmlCompleto);
        var chaveNotaFiscal = string.IsNullOrWhiteSpace(dadosNfe?.ChaveAcesso)
            ? documentoNfe.ChaveAcesso
            : dadosNfe.ChaveAcesso;

        var notaFiscal = new NotaFiscal(
            recebimento.Id,
            arquivoId: null,
            numeroNota: dadosNfe?.NumeroNota ?? documentoNfe.NumeroNota,
            serie: dadosNfe?.Serie ?? documentoNfe.Serie,
            chaveAcesso: chaveNotaFiscal,
            valorTotal: dadosNfe?.ValorTotal ?? documentoNfe.ValorTotal ?? 0m,
            dataEmissao: dadosNfe?.DataEmissao ?? documentoNfe.DataEmissao,
            dataEntrada: DateTime.UtcNow,
            cpfCnpjEmitente: dadosNfe?.CnpjEmitente ?? documentoNfe.CnpjEmitente,
            nomeEmitente: dadosNfe?.NomeEmitente ?? documentoNfe.NomeEmitente,
            observacao: request.Observacao);

        if (!notaFiscal.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", notaFiscal.Errors));
        }

        await notaFiscalRepository.Create(notaFiscal, cancellationToken);

        await CriarHistoricoAsync(
            recebimento.Id,
            request.UsuarioId,
            AcoesHistorico.NotaFiscalVinculada,
            $"NF-e {chaveAcesso} vinculada ao recebimento.",
            cancellationToken);

        var criado = await recebimentoRepository.GetDetailsByIdAsync(recebimento.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos o cadastro.");

        return MapToDetalhe(criado);
    }

    public async Task<MobileRecebimentoDetalheResponse?> IniciarConferenciaAsync(
        ulong recebimentoId,
        ulong usuarioId,
        string? observacao,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken);
        if (recebimento is null)
        {
            return null;
        }

        EnsureRecebimentoPertenceUsuario(recebimento, usuarioId);
        await EnsureUsuarioExistsAsync(usuarioId, cancellationToken);

        if (recebimento.StatusRecebimento == RecebimentoStatusEnum.EmConferencia)
        {
            return MapToDetalhe(recebimento);
        }

        recebimento.IniciarConferencia(observacao);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Update(recebimento, cancellationToken);

        await CriarHistoricoAsync(
            recebimento.Id,
            usuarioId,
            AcoesHistorico.ConferenciaIniciada,
            $"Conferencia iniciada pelo aplicativo mobile. Status alterado para {(int)RecebimentoStatusEnum.EmConferencia} - {RecebimentoStatusEnum.EmConferencia}.",
            cancellationToken);

        var atualizado = await recebimentoRepository.GetDetailsByIdAsync(recebimento.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos iniciar a conferencia.");

        return MapToDetalhe(atualizado);
    }

    public async Task<MobileRecebimentoDetalheResponse?> FinalizarConferenciaAsync(
        ulong recebimentoId,
        ulong usuarioId,
        MobileFinalizarConferenciaRequest request,
        CancellationToken cancellationToken = default)
    {
        var recebimento = await recebimentoRepository.GetDetailsByIdAsync(recebimentoId, cancellationToken);
        if (recebimento is null)
        {
            return null;
        }

        EnsureRecebimentoPertenceUsuario(recebimento, usuarioId);
        await EnsureUsuarioExistsAsync(usuarioId, cancellationToken);

        var novoStatus = request.PossuiDivergencia
            ? RecebimentoStatusEnum.ComDivergencia
            : RecebimentoStatusEnum.Conferido;

        if (recebimento.StatusRecebimento == novoStatus)
        {
            return MapToDetalhe(recebimento);
        }

        var statusConferencia = request.PossuiDivergencia
            ? StatusConferencia.Reprovada
            : StatusConferencia.Aprovada;

        var conferencia = new RecebimentoConferencia(
            recebimentoId,
            usuarioId,
            statusConferencia,
            notaEncontrada: true,
            boletoEncontrado: true,
            valorConfere: true,
            dataVencimentoConfere: true,
            documentoLegivel: true,
            request.Observacao);

        if (!conferencia.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", conferencia.Errors));
        }

        recebimento.FinalizarConferencia(novoStatus, request.Observacao);

        if (!recebimento.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", recebimento.Errors));
        }

        await recebimentoRepository.Update(recebimento, cancellationToken);
        await recebimentoConferenciaRepository.Create(conferencia, cancellationToken);

        await CriarHistoricoAsync(
            recebimento.Id,
            usuarioId,
            AcoesHistorico.ConferenciaRealizada,
            $"Conferencia de produtos finalizada pelo aplicativo mobile com status {(int)novoStatus} - {novoStatus}.",
            cancellationToken);

        var atualizado = await recebimentoRepository.GetDetailsByIdAsync(recebimento.Id, cancellationToken)
            ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos finalizar a conferencia.");

        return MapToDetalhe(atualizado);
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

    private async Task EnsureUsuarioExistsAsync(ulong usuarioId, CancellationToken cancellationToken)
    {
        if (!await usuarioRepository.RecordExists(usuarioId, cancellationToken))
        {
            throw new RegraDeNegocioException("Usuario nao encontrado.");
        }
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

    private static void EnsureRecebimentoPertenceUsuario(Recebimento recebimento, ulong usuarioId)
    {
        if (recebimento.UsuarioEnvioId != usuarioId)
        {
            throw new RegraDeNegocioException("Acesso negado ao recebimento informado.");
        }
    }

    private async Task CriarHistoricoAsync(
        ulong recebimentoId,
        ulong usuarioId,
        string acao,
        string descricao,
        CancellationToken cancellationToken)
    {
        var historico = new RecebimentoHistorico(recebimentoId, usuarioId, acao, descricao);

        if (!historico.Validate())
        {
            throw new RegraDeNegocioException(string.Join(" ", historico.Errors));
        }

        await recebimentoHistoricoRepository.Create(historico, cancellationToken);
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

        var notasFiscais = recebimento.NotasFiscais
            .Select(nota => new MobileNotaFiscalResponse(
                nota.Id,
                nota.NumeroNota,
                nota.Serie,
                nota.ChaveAcesso,
                nota.ValorTotal,
                nota.DataEmissao,
                nota.DataEntrada,
                nota.CpfCnpjEmitente,
                nota.NomeEmitente,
                nota.Observacao))
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
            notasFiscais,
            documentos,
            historico);
    }
}
