using System.Security.Cryptography.X509Certificates;
using Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;
using Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Enums;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Nfe.Services;

public sealed class NfeAppService(
    IEmpresaRepository empresaRepository,
    INfeDocumentoRepository nfeDocumentoRepository,
    INfeProdutoRepository nfeProdutoRepository,
    ISefazNfeClient sefazNfeClient,
    ICertificadoDigitalProvider certificadoDigitalProvider) : INfeAppService
{
    private const string CodigoStatusDocumentosLocalizados = "138";
    private const string CodigoStatusNenhumDocumento = "137";
    private const string CodigoStatusConsumoIndevido = "656";
    private const string CodigoStatusEventoRegistrado = "135";
    private const string CodigoStatusEventoRegistradoSemVinculo = "136";
    private const string CodigoStatusDuplicidadeEvento = "573";

    private static readonly TimeSpan[] IntervalosConsultaXmlAposCiencia =
    [
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(20),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(40)
    ];

    public async Task AtualizarCertificadoDigitalAsync(
        AtualizarCertificadoDigitalEmpresaRequest request,
        CancellationToken cancellationToken = default)
    {
        var empresa = await empresaRepository.Get(request.EmpresaId, cancellationToken)
            ?? throw new RegraDeNegocioException("Empresa nao encontrada.");

        byte[] certificadoBytes;
        try
        {
            certificadoBytes = Convert.FromBase64String(request.CertificadoBase64);
        }
        catch
        {
            throw new RegraDeNegocioException("O certificado digital informado nao e um Base64 valido.");
        }

        var (valido, mensagemErro) = certificadoDigitalProvider.ValidarCertificado(
            certificadoBytes,
            request.SenhaCertificado,
            empresa.Cnpj);

        if (!valido)
        {
            throw new RegraDeNegocioException(mensagemErro ?? "Certificado digital invalido.");
        }

        var certificado = certificadoDigitalProvider.CarregarCertificado(certificadoBytes, request.SenhaCertificado);
        var senhaCriptografada = certificadoDigitalProvider.CriptografarSenha(request.SenhaCertificado);

        empresa.AtualizarCertificadoDigital(
            certificadoBytes,
            senhaCriptografada,
            certificado.NotAfter.ToUniversalTime(),
            request.CodigoUf,
            request.CertificadoDigitalAtivo);

        await empresaRepository.Update(empresa, cancellationToken);
    }

    public async Task<BuscarNfeResponse> BuscarPorChaveAcessoAsync(
        BuscarNfeRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ChaveAcesso);

        if (request.ChaveAcesso.Length != 44)
            throw new RegraDeNegocioException("A chave de acesso deve conter 44 digitos.");

        var empresa = await empresaRepository.Get(request.EmpresaId, cancellationToken)
            ?? throw new RegraDeNegocioException("Empresa nao encontrada.");

        if (empresa.CertificadoDigitalA1 is null || string.IsNullOrWhiteSpace(empresa.CertificadoDigitalSenha))
            throw new RegraDeNegocioException("A empresa nao possui certificado digital cadastrado.");

        if (!empresa.CertificadoDigitalAtivo)
            throw new RegraDeNegocioException("O certificado digital da empresa esta inativo.");

        if (empresa.CertificadoDigitalValidade.HasValue && empresa.CertificadoDigitalValidade.Value < DateTime.UtcNow)
            throw new RegraDeNegocioException("O certificado digital da empresa esta vencido.");

        var senhaDecriptografada = certificadoDigitalProvider.DescriptografarSenha(empresa.CertificadoDigitalSenha);
        var certificado = certificadoDigitalProvider.CarregarCertificado(empresa.CertificadoDigitalA1, senhaDecriptografada);

        var documentoLocal = await nfeDocumentoRepository.GetWithProdutosByChaveAcessoAsync(request.ChaveAcesso, cancellationToken);
        if (documentoLocal is not null)
        {
            if (documentoLocal.EmpresaId != empresa.Id)
                throw new RegraDeNegocioException("Acesso negado. Esta nota nao pertence a empresa informada.");

            if (!string.IsNullOrWhiteSpace(documentoLocal.XmlCompleto) && documentoLocal.Produtos.Any())
                return MapToBuscarResponse(documentoLocal, documentoLocal.Produtos.Select(MapProdutoToResponse).ToList());
        }

        // Consulta SEFAZ diretamente pela chave de acesso (consChNFe)
        var resultado = await sefazNfeClient.ConsultarPorChaveAcessoAsync(
            certificado, empresa.Cnpj, empresa.NfeCodigoUf, request.ChaveAcesso, cancellationToken);

        if (resultado.CodigoStatus == CodigoStatusConsumoIndevido)
            throw new RegraDeNegocioException("Consulta rejeitada pela SEFAZ por consumo indevido. Aguarde antes de tentar novamente.");

        if (resultado.Documentos.Count == 0)
            throw new RegraDeNegocioException(
                "Nota fiscal nao encontrada na SEFAZ. Verifique se sua empresa e o destinatario desta nota.");

        var docSefaz = resultado.Documentos[0];

        // SEFAZ retornou o XML completo — processa e retorna com produtos
        if (docSefaz.TipoDocumento == TipoDocumentoFiscalEnum.NfeCompleta)
        {
            return await ProcessarXmlCompletoAsync(
                empresa, request.ChaveAcesso, docSefaz.XmlDescomprimido, cancellationToken);
        }

        // SEFAZ retornou resumo. Verifica se já demos ciência antes (banco de dados)
        var notaNoDb = await nfeDocumentoRepository.GetByChaveAcessoAsync(request.ChaveAcesso, cancellationToken);

        var possuiManifestacaoLocal = notaNoDb?.StatusManifestacao
            is StatusManifestacaoNfeEnum.CienciaOperacao
            or StatusManifestacaoNfeEnum.ConfirmacaoOperacao
            or StatusManifestacaoNfeEnum.DesconhecimentoOperacao
            or StatusManifestacaoNfeEnum.OperacaoNaoRealizada;

        var possuiRetornoSefaz = !string.IsNullOrWhiteSpace(notaNoDb?.RetornoSefaz)
            || !string.IsNullOrWhiteSpace(notaNoDb?.ProtocoloManifestacao);

        var manifestacaoFinal = notaNoDb?.StatusManifestacao
            is StatusManifestacaoNfeEnum.ConfirmacaoOperacao
            or StatusManifestacaoNfeEnum.DesconhecimentoOperacao
            or StatusManifestacaoNfeEnum.OperacaoNaoRealizada;

        var cienciaJaDada = manifestacaoFinal || (possuiManifestacaoLocal && possuiRetornoSefaz);

        if (!cienciaJaDada)
        {
            // Dá ciência pela primeira vez e salva no banco
            var docResumo = docSefaz;
            NfeDocumento registro;

            if (notaNoDb is null)
            {
                registro = new NfeDocumento(
                    empresa.Id, request.ChaveAcesso, null,
                    TipoDocumentoFiscalEnum.ResumoNfe, null,
                    xmlResumo: docResumo.XmlDescomprimido, xmlCompleto: null);

                if (registro.Validate())
                {
                    try { await nfeDocumentoRepository.Create(registro, cancellationToken); }
                    catch (Exception ex) when (EhDuplicata(ex))
                    {
                        registro = await nfeDocumentoRepository.GetByChaveAcessoAsync(request.ChaveAcesso, cancellationToken)
                            ?? registro;
                    }
                }
            }
            else
            {
                registro = notaNoDb;
            }

            var resultadoManifestacao = await sefazNfeClient.EnviarManifestacaoAsync(
                certificado, empresa.Cnpj, empresa.NfeCodigoUf,
                request.ChaveAcesso, TipoManifestacaoNfeEnum.CienciaOperacao,
                null, cancellationToken);

            if (!ManifestacaoAceita(resultadoManifestacao))
            {
                registro.RegistrarManifestacao(
                    StatusManifestacaoNfeEnum.Rejeitado,
                    null,
                    resultadoManifestacao.XmlRetorno);
                await nfeDocumentoRepository.Update(registro, cancellationToken);

                throw new RegraDeNegocioException(
                    $"Manifestacao rejeitada pela SEFAZ: {resultadoManifestacao.CodigoStatus} - {resultadoManifestacao.Motivo}");
            }

            registro.RegistrarManifestacao(
                StatusManifestacaoNfeEnum.CienciaOperacao,
                resultadoManifestacao.Protocolo,
                resultadoManifestacao.XmlRetorno);
            await nfeDocumentoRepository.Update(registro, cancellationToken);
        }

        // Segunda tentativa pela mesma chave após a ciência
        var xmlCompleto = await AguardarXmlCompletoAposCienciaAsync(
            certificado,
            empresa.Cnpj,
            empresa.NfeCodigoUf,
            request.ChaveAcesso,
            cancellationToken);

        if (xmlCompleto is not null)
            return await ProcessarXmlCompletoAsync(empresa, request.ChaveAcesso, xmlCompleto, cancellationToken);

        // SEFAZ ainda processa — na próxima chamada a ciência não será repetida
        throw new RegraDeNegocioException(
            "Ciencia registrada, mas a SEFAZ ainda nao liberou o XML completo. Aguarde alguns minutos e tente novamente.");
    }

    public async Task<SincronizarNfeResponse> SincronizarAsync(
        SincronizarNfeRequest request,
        CancellationToken cancellationToken = default)
    {
        var empresa = await empresaRepository.Get(request.EmpresaId, cancellationToken)
            ?? throw new RegraDeNegocioException("Empresa nao encontrada.");

        if (empresa.CertificadoDigitalA1 is null || string.IsNullOrWhiteSpace(empresa.CertificadoDigitalSenha))
        {
            throw new RegraDeNegocioException("A empresa nao possui certificado digital cadastrado.");
        }

        if (!empresa.CertificadoDigitalAtivo)
        {
            throw new RegraDeNegocioException("O certificado digital da empresa esta inativo.");
        }

        if (empresa.CertificadoDigitalValidade.HasValue && empresa.CertificadoDigitalValidade.Value < DateTime.UtcNow)
        {
            throw new RegraDeNegocioException("O certificado digital da empresa esta vencido.");
        }

        var senhaDecriptografada = certificadoDigitalProvider.DescriptografarSenha(empresa.CertificadoDigitalSenha);
        var certificado = certificadoDigitalProvider.CarregarCertificado(empresa.CertificadoDigitalA1, senhaDecriptografada);

        var resultado = await sefazNfeClient.ConsultarDistribuicaoDFeAsync(
            certificado,
            empresa.Cnpj,
            empresa.NfeCodigoUf,
            empresa.NfeUltimoNsu,
            cancellationToken);

        if (resultado.CodigoStatus == CodigoStatusConsumoIndevido)
        {
            throw new RegraDeNegocioException(
                "Consulta rejeitada pela SEFAZ por consumo indevido. Aguarde antes de tentar novamente.");
        }

        var documentosProcessados = 0;

        if (resultado.CodigoStatus == CodigoStatusDocumentosLocalizados && resultado.Documentos.Count > 0)
        {
            documentosProcessados = await ProcessarDocumentosSefazAsync(
                empresa.Id,
                empresa.Cnpj,
                resultado.Documentos,
                cancellationToken);
        }

        empresa.AtualizarNsu(resultado.UltimoNsu, resultado.MaxNsu);
        await empresaRepository.Update(empresa, cancellationToken);

        var mensagem = resultado.CodigoStatus == CodigoStatusNenhumDocumento
            ? "Nenhum documento novo localizado."
            : $"Sincronizacao concluida. {resultado.Motivo}";

        return new SincronizarNfeResponse(
            Sucesso: true,
            Mensagem: mensagem,
            QuantidadeDocumentosProcessados: documentosProcessados,
            UltimoNsu: resultado.UltimoNsu.ToString("D15"),
            MaxNsu: resultado.MaxNsu.ToString("D15"),
            ProximaConsultaPermitidaEm: DateTime.UtcNow);
    }

    public async Task<NfeDocumentoResponse?> GetByChaveAcessoAsync(
        string chaveAcesso,
        ulong empresaId,
        CancellationToken cancellationToken = default)
    {
        var documento = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken);

        if (documento is null) return null;

        if (documento.EmpresaId != empresaId)
        {
            throw new RegraDeNegocioException("Acesso negado. Esta nota nao pertence a empresa informada.");
        }

        return MapToResponse(documento);
    }

    public async Task<string> GetXmlByChaveAcessoAsync(
        string chaveAcesso,
        ulong empresaId,
        CancellationToken cancellationToken = default)
    {
        var documento = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken)
            ?? throw new RegraDeNegocioException("Nota fiscal nao encontrada.");

        if (documento.EmpresaId != empresaId)
        {
            throw new RegraDeNegocioException("Acesso negado. Esta nota nao pertence a empresa informada.");
        }

        if (string.IsNullOrWhiteSpace(documento.XmlCompleto))
        {
            throw new RegraDeNegocioException(
                "O XML completo desta nota ainda nao esta disponivel. Realize a manifestacao e sincronize novamente.");
        }

        return documento.XmlCompleto;
    }

    public async Task<List<NfeProdutoResponse>> GetProdutosByChaveAcessoAsync(
        string chaveAcesso,
        ulong empresaId,
        CancellationToken cancellationToken = default)
    {
        var documento = await nfeDocumentoRepository.GetWithProdutosByChaveAcessoAsync(chaveAcesso, cancellationToken)
            ?? throw new RegraDeNegocioException("Nota fiscal nao encontrada.");

        if (documento.EmpresaId != empresaId)
        {
            throw new RegraDeNegocioException("Acesso negado. Esta nota nao pertence a empresa informada.");
        }

        if (!documento.Produtos.Any())
        {
            throw new RegraDeNegocioException(
                "Nenhum produto encontrado para esta nota. Verifique se o XML completo foi processado.");
        }

        return documento.Produtos.Select(MapProdutoToResponse).ToList();
    }

    public async Task<NfeDocumentoResponse> ManifestarAsync(
        string chaveAcesso,
        ManifestarNfeRequest request,
        CancellationToken cancellationToken = default)
    {
        var documento = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken)
            ?? throw new RegraDeNegocioException("Nota fiscal nao encontrada.");

        if (documento.EmpresaId != request.EmpresaId)
        {
            throw new RegraDeNegocioException("Acesso negado. Esta nota nao pertence a empresa informada.");
        }

        var empresa = await empresaRepository.Get(request.EmpresaId, cancellationToken)
            ?? throw new RegraDeNegocioException("Empresa nao encontrada.");

        if (empresa.CertificadoDigitalA1 is null || string.IsNullOrWhiteSpace(empresa.CertificadoDigitalSenha))
        {
            throw new RegraDeNegocioException("A empresa nao possui certificado digital cadastrado.");
        }

        if (empresa.CertificadoDigitalValidade.HasValue && empresa.CertificadoDigitalValidade.Value < DateTime.UtcNow)
        {
            throw new RegraDeNegocioException("O certificado digital da empresa esta vencido.");
        }

        var senhaDecriptografada = certificadoDigitalProvider.DescriptografarSenha(empresa.CertificadoDigitalSenha);
        var certificado = certificadoDigitalProvider.CarregarCertificado(empresa.CertificadoDigitalA1, senhaDecriptografada);

        var resultadoManifestacao = await sefazNfeClient.EnviarManifestacaoAsync(
            certificado,
            empresa.Cnpj,
            empresa.NfeCodigoUf,
            chaveAcesso,
            request.TipoManifestacao,
            request.Justificativa,
            cancellationToken);

        var novoStatus = request.TipoManifestacao switch
        {
            TipoManifestacaoNfeEnum.CienciaOperacao => StatusManifestacaoNfeEnum.CienciaOperacao,
            TipoManifestacaoNfeEnum.ConfirmacaoOperacao => StatusManifestacaoNfeEnum.ConfirmacaoOperacao,
            TipoManifestacaoNfeEnum.DesconhecimentoOperacao => StatusManifestacaoNfeEnum.DesconhecimentoOperacao,
            TipoManifestacaoNfeEnum.OperacaoNaoRealizada => StatusManifestacaoNfeEnum.OperacaoNaoRealizada,
            _ => throw new RegraDeNegocioException("Tipo de manifestacao invalido.")
        };

        if (resultadoManifestacao.CodigoStatus is not ("135" or "136"))
        {
            documento.RegistrarManifestacao(
                StatusManifestacaoNfeEnum.Rejeitado,
                null,
                resultadoManifestacao.XmlRetorno);
        }
        else
        {
            documento.RegistrarManifestacao(
                novoStatus,
                resultadoManifestacao.Protocolo,
                resultadoManifestacao.XmlRetorno);
        }

        await nfeDocumentoRepository.Update(documento, cancellationToken);

        if (resultadoManifestacao.CodigoStatus is not ("135" or "136"))
        {
            throw new RegraDeNegocioException(
                $"Manifestacao rejeitada pela SEFAZ: {resultadoManifestacao.Motivo}");
        }

        // Sinaliza que a SEFAZ tem o procNFe disponível para download.
        // Isso garante que a próxima sync não seja bloqueada pelo cooldown.
        empresa.SinalizarSyncPendente();
        await empresaRepository.Update(empresa, cancellationToken);

        return MapToResponse(documento);
    }

    private async Task<int> ProcessarDocumentosSefazAsync(
        ulong empresaId,
        string cnpjEmpresa,
        IReadOnlyList<SefazDocumento> documentos,
        CancellationToken cancellationToken)
    {
        var processados = 0;

        foreach (var docSefaz in documentos)
        {
            var tipoDocumento = NfeXmlParser.ResolverTipoDocumentoPorSchema(docSefaz.Schema);
            var isNfeCompleta = tipoDocumento == TipoDocumentoFiscalEnum.NfeCompleta;
            var isResumo = tipoDocumento == TipoDocumentoFiscalEnum.ResumoNfe;

            if (!isNfeCompleta && !isResumo) continue;

            var chaveAcesso = isNfeCompleta
                ? ObterChaveAcessoDoXmlCompleto(docSefaz.XmlDescomprimido)
                : NfeXmlParser.ExtrairChaveAcessoDoResumo(docSefaz.XmlDescomprimido);

            if (string.IsNullOrWhiteSpace(chaveAcesso) || chaveAcesso.Length != 44) continue;

            var documentoExistente = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken);

            if (documentoExistente is not null)
            {
                if (isNfeCompleta && string.IsNullOrWhiteSpace(documentoExistente.XmlCompleto))
                {
                    documentoExistente.SalvarXmlCompleto(docSefaz.XmlDescomprimido);
                    await AtualizarDadosEProdutosAsync(documentoExistente, docSefaz.XmlDescomprimido, cnpjEmpresa, cancellationToken);
                    processados++;
                }
                continue;
            }

            var novoDocumento = new NfeDocumento(
                empresaId,
                chaveAcesso,
                docSefaz.Nsu,
                tipoDocumento,
                docSefaz.Schema,
                xmlResumo: isResumo ? docSefaz.XmlDescomprimido : null,
                xmlCompleto: isNfeCompleta ? docSefaz.XmlDescomprimido : null);

            if (!novoDocumento.Validate())
            {
                throw new RegraDeNegocioException(string.Join(" ", novoDocumento.Errors));
            }

            if (isNfeCompleta)
            {
                var dados = NfeXmlParser.ParsearNfeCompleta(docSefaz.XmlDescomprimido);
                if (dados is not null)
                    novoDocumento.AtualizarDadosNota(
                        dados.CnpjEmitente, dados.NomeEmitente,
                        dados.CnpjDestinatario, dados.NomeDestinatario,
                        dados.NumeroNota, dados.Serie,
                        dados.DataEmissao, dados.ValorTotal);
            }
            else if (isResumo)
            {
                var dadosRes = NfeXmlParser.ParsearResumoNfe(docSefaz.XmlDescomprimido);
                if (dadosRes is not null)
                    novoDocumento.AtualizarDadosNota(
                        dadosRes.CnpjEmitente, dadosRes.NomeEmitente,
                        dadosRes.CnpjDestinatario, null,
                        null, null, dadosRes.DataEmissao, dadosRes.ValorTotal);
            }

            try
            {
                await nfeDocumentoRepository.Create(novoDocumento, cancellationToken);
            }
            catch (Exception ex) when (EhDuplicata(ex))
            {
                // Documento já foi inserido em sync anterior que falhou antes de atualizar o NSU.
                // Ignora e avança para o próximo documento da lista.
                continue;
            }

            if (isNfeCompleta)
            {
                await SalvarProdutosAsync(novoDocumento.Id, docSefaz.XmlDescomprimido, cancellationToken);
            }

            processados++;
        }

        return processados;
    }

    private static bool EhDuplicata(Exception ex)
    {
        const string marcador = "Duplicate entry";
        return ex.Message.Contains(marcador, StringComparison.OrdinalIgnoreCase)
            || ex.InnerException?.Message.Contains(marcador, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool ManifestacaoAceita(SefazManifestacaoResult resultado)
    {
        return resultado.CodigoStatus is CodigoStatusEventoRegistrado
            or CodigoStatusEventoRegistradoSemVinculo
            or CodigoStatusDuplicidadeEvento;
    }

    private async Task<string?> AguardarXmlCompletoAposCienciaAsync(
        X509Certificate2 certificado,
        string cnpj,
        int codigoUf,
        string chaveAcesso,
        CancellationToken cancellationToken)
    {
        foreach (var intervalo in IntervalosConsultaXmlAposCiencia)
        {
            if (intervalo > TimeSpan.Zero)
                await Task.Delay(intervalo, cancellationToken);

            var resultado = await sefazNfeClient.ConsultarPorChaveAcessoAsync(
                certificado,
                cnpj,
                codigoUf,
                chaveAcesso,
                cancellationToken);

            if (resultado.CodigoStatus == CodigoStatusConsumoIndevido)
                throw new RegraDeNegocioException("Consulta rejeitada pela SEFAZ por consumo indevido. Aguarde antes de tentar novamente.");

            var documentoCompleto = resultado.Documentos
                .FirstOrDefault(d => d.TipoDocumento == TipoDocumentoFiscalEnum.NfeCompleta);

            if (documentoCompleto is not null)
                return documentoCompleto.XmlDescomprimido;
        }

        return null;
    }

    private async Task AtualizarDadosEProdutosAsync(
        NfeDocumento documento,
        string xmlCompleto,
        string cnpjEmpresa,
        CancellationToken cancellationToken)
    {
        var dados = NfeXmlParser.ParsearNfeCompleta(xmlCompleto);
        if (dados is not null)
        {
            documento.AtualizarDadosNota(
                dados.CnpjEmitente,
                dados.NomeEmitente,
                dados.CnpjDestinatario,
                dados.NomeDestinatario,
                dados.NumeroNota,
                dados.Serie,
                dados.DataEmissao,
                dados.ValorTotal);
        }

        await nfeDocumentoRepository.Update(documento, cancellationToken);
        await SalvarProdutosAsync(documento.Id, xmlCompleto, cancellationToken);
    }

    private async Task SalvarProdutosAsync(
        ulong nfeDocumentoId,
        string xmlCompleto,
        CancellationToken cancellationToken)
    {
        var dados = NfeXmlParser.ParsearNfeCompleta(xmlCompleto);
        if (dados is null || dados.Produtos.Count == 0) return;

        var produtos = dados.Produtos.Select(p => new NfeProduto(
            nfeDocumentoId,
            p.NomeProduto,
            p.CodigoProduto,
            descricao: null,
            p.Ncm,
            p.Cfop,
            p.Unidade,
            p.Quantidade,
            p.ValorUnitario,
            p.ValorTotal,
            p.Ean)).ToList();

        foreach (var produto in produtos.Where(p => !p.Validate()))
        {
            throw new RegraDeNegocioException(string.Join(" ", produto.Errors));
        }

        await nfeProdutoRepository.CreateRange(produtos, cancellationToken);
    }

    private static string? ObterChaveAcessoDoXmlCompleto(string xml)
    {
        var dados = NfeXmlParser.ParsearNfeCompleta(xml);
        return dados?.ChaveAcesso;
    }

    private async Task<BuscarNfeResponse> ProcessarXmlCompletoAsync(
        Domain.Modules.Empresas.Entities.Empresa empresa,
        string chaveAcesso,
        string xmlCompleto,
        CancellationToken cancellationToken)
    {
        var dados = NfeXmlParser.ParsearNfeCompleta(xmlCompleto);
        var docExistente = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken);

        NfeDocumento doc;
        if (docExistente is not null)
        {
            doc = docExistente;
            doc.SalvarXmlCompleto(xmlCompleto);
            if (dados is not null)
                doc.AtualizarDadosNota(
                    dados.CnpjEmitente, dados.NomeEmitente,
                    dados.CnpjDestinatario, dados.NomeDestinatario,
                    dados.NumeroNota, dados.Serie, dados.DataEmissao, dados.ValorTotal);
            await nfeDocumentoRepository.Update(doc, cancellationToken);
        }
        else
        {
            doc = new NfeDocumento(
                empresa.Id, chaveAcesso, null,
                TipoDocumentoFiscalEnum.NfeCompleta, null,
                xmlResumo: null, xmlCompleto: xmlCompleto);

            if (dados is not null)
                doc.AtualizarDadosNota(
                    dados.CnpjEmitente, dados.NomeEmitente,
                    dados.CnpjDestinatario, dados.NomeDestinatario,
                    dados.NumeroNota, dados.Serie, dados.DataEmissao, dados.ValorTotal);

            if (!doc.Validate())
                throw new RegraDeNegocioException(string.Join(" ", doc.Errors));

            try { await nfeDocumentoRepository.Create(doc, cancellationToken); }
            catch (Exception ex) when (EhDuplicata(ex))
            {
                doc = await nfeDocumentoRepository.GetByChaveAcessoAsync(chaveAcesso, cancellationToken)
                    ?? doc;
            }
        }

        var produtosExistentes = await nfeProdutoRepository.GetByNfeDocumentoIdAsync(doc.Id, cancellationToken);
        if (produtosExistentes.Count > 0)
            return MapToBuscarResponse(doc, produtosExistentes.Select(MapProdutoToResponse).ToList());

        var produtosXml = dados?.Produtos ?? [];
        var produtos = produtosXml.Select(p => new NfeProduto(
            doc.Id, p.NomeProduto, p.CodigoProduto, null,
            p.Ncm, p.Cfop, p.Unidade, p.Quantidade, p.ValorUnitario, p.ValorTotal, p.Ean)).ToList();

        if (produtos.Count > 0)
        {
            foreach (var prod in produtos.Where(p => !p.Validate()))
                throw new RegraDeNegocioException(string.Join(" ", prod.Errors));

            try { await nfeProdutoRepository.CreateRange(produtos, cancellationToken); }
            catch (Exception ex) when (EhDuplicata(ex)) { }
        }

        return MapToBuscarResponse(doc, produtos.Select(MapProdutoToResponse).ToList());
    }

    private static BuscarNfeResponse MapToBuscarResponse(NfeDocumento documento, IReadOnlyList<NfeProdutoResponse> produtos)
    {
        return new BuscarNfeResponse(
            documento.Id,
            documento.EmpresaId,
            documento.ChaveAcesso,
            documento.Nsu,
            documento.TipoDocumento.ToString(),
            documento.CnpjEmitente,
            documento.NomeEmitente,
            documento.CnpjDestinatario,
            documento.NomeDestinatario,
            documento.NumeroNota,
            documento.Serie,
            documento.DataEmissao,
            documento.ValorTotal,
            documento.StatusManifestacao.ToString(),
            documento.DataDownload,
            documento.DataCadastro,
            documento.DataAtualizacao,
            produtos);
    }

    private static NfeDocumentoResponse MapToResponse(NfeDocumento documento)
    {
        return new NfeDocumentoResponse(
            documento.Id,
            documento.EmpresaId,
            documento.ChaveAcesso,
            documento.Nsu,
            documento.TipoDocumento.ToString(),
            documento.CnpjEmitente,
            documento.NomeEmitente,
            documento.CnpjDestinatario,
            documento.NomeDestinatario,
            documento.NumeroNota,
            documento.Serie,
            documento.DataEmissao,
            documento.ValorTotal,
            documento.StatusManifestacao.ToString(),
            documento.DataDownload,
            documento.DataCadastro,
            documento.DataAtualizacao);
    }

    private static NfeProdutoResponse MapProdutoToResponse(NfeProduto produto)
    {
        return new NfeProdutoResponse(
            produto.Id,
            produto.NfeDocumentoId,
            produto.CodigoProduto,
            produto.NomeProduto,
            produto.Descricao,
            produto.Ncm,
            produto.Cfop,
            produto.Unidade,
            produto.Quantidade,
            produto.ValorUnitario,
            produto.ValorTotal,
            produto.Ean);
    }
}
