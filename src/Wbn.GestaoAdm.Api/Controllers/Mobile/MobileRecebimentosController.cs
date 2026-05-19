using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Recebimentos.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;

namespace Wbn.GestaoAdm.Api.Controllers.Mobile;

[ApiController]
[Authorize]
[Route("api/mobile/recebimentos")]
public sealed class MobileRecebimentosController(
    IMobileRecebimentoAppService recebimentoAppService,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("meus-envios")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MobileRecebimentoSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MobileRecebimentoSummaryResponse>>> GetMeusEnvios(
        CancellationToken cancellationToken)
    {
        var usuarioId = ObterUsuarioId();
        var envios = await recebimentoAppService.GetMeusEnviosAsync(usuarioId, cancellationToken);
        return Ok(envios);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(MobileRecebimentoDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MobileRecebimentoDetalheResponse>> GetDetalhe(
        ulong id,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            var detalhe = await recebimentoAppService.GetDetalheAsync(id, usuarioId, cancellationToken);

            if (detalhe is null)
            {
                return NotFound();
            }

            return Ok(detalhe);
        }
        catch (RegraDeNegocioException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(MobileRecebimentoDetalheResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MobileRecebimentoDetalheResponse>> Create(
        [FromBody] MobileCriarRecebimentoBody body,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            var request = new MobileCreateRecebimentoRequest(body.EmpresaId, usuarioId, body.Observacao);
            var recebimento = await recebimentoAppService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetDetalhe),
                new { id = recebimento.Id },
                recebimento);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("nfe")]
    [ProducesResponseType(typeof(MobileRecebimentoDetalheResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MobileRecebimentoDetalheResponse>> CreateFromNfe(
        [FromBody] MobileCriarRecebimentoNfeBody body,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            var request = new MobileCreateRecebimentoNfeRequest(
                body.EmpresaId,
                usuarioId,
                body.ChaveAcesso,
                body.Observacao);

            var recebimento = await recebimentoAppService.CreateFromNfeAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetDetalhe),
                new { id = recebimento.Id },
                recebimento);
        }
        catch (RegraDeNegocioException ex) when (ex.Message.Contains("Acesso", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}/iniciar-conferencia")]
    [ProducesResponseType(typeof(MobileRecebimentoDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MobileRecebimentoDetalheResponse>> IniciarConferencia(
        ulong id,
        [FromBody] MobileIniciarConferenciaBody? body,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            var recebimento = await recebimentoAppService.IniciarConferenciaAsync(
                id,
                usuarioId,
                body?.Observacao,
                cancellationToken);

            return recebimento is null ? NotFound() : Ok(recebimento);
        }
        catch (RegraDeNegocioException ex) when (ex.Message.Contains("Acesso", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:long}/finalizar-conferencia")]
    [ProducesResponseType(typeof(MobileRecebimentoDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MobileRecebimentoDetalheResponse>> FinalizarConferencia(
        ulong id,
        [FromBody] MobileFinalizarConferenciaBody body,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            var request = new MobileFinalizarConferenciaRequest(body.PossuiDivergencia, body.Observacao);
            var recebimento = await recebimentoAppService.FinalizarConferenciaAsync(
                id,
                usuarioId,
                request,
                cancellationToken);

            return recebimento is null ? NotFound() : Ok(recebimento);
        }
        catch (RegraDeNegocioException ex) when (ex.Message.Contains("Acesso", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{recebimentoId:long}/documentos")]
    [ProducesResponseType(typeof(MobileDocumentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MobileDocumentoResponse>> EnviarDocumento(
        ulong recebimentoId,
        [FromForm] MobileEnviarDocumentoBody body,
        CancellationToken cancellationToken)
    {
        if (body.Arquivo is null || body.Arquivo.Length == 0)
        {
            return BadRequest(new { message = "Nenhum arquivo foi enviado." });
        }

        try
        {
            var usuarioId = ObterUsuarioId();
            var (nomeArquivo, caminho) = await SalvarArquivoAsync(recebimentoId, body.Arquivo);

            var request = new MobileEnviarDocumentoRequest(
                recebimentoId,
                body.TipoDocumentoId,
                usuarioId,
                body.Arquivo.FileName,
                nomeArquivo,
                caminho,
                Path.GetExtension(body.Arquivo.FileName).TrimStart('.'),
                body.Arquivo.ContentType,
                body.Arquivo.Length,
                body.OrdemExibicao);

            var documento = await recebimentoAppService.EnviarDocumentoAsync(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, documento);
        }
        catch (RegraDeNegocioException ex) when (ex.Message.Contains("Acesso"))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("com-documentos")]
    [ProducesResponseType(typeof(MobileRecebimentoDetalheResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MobileRecebimentoDetalheResponse>> CreateComDocumentos(
        [FromForm] MobileCriarComDocumentosBody body,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();

            var request = new MobileCreateRecebimentoRequest(body.EmpresaId, usuarioId, body.Observacao);
            var recebimento = await recebimentoAppService.CreateAsync(request, cancellationToken);

            foreach (var docBody in body.Documentos ?? [])
            {
                if (docBody.Arquivo is null || docBody.Arquivo.Length == 0)
                {
                    continue;
                }

                var (nomeArquivo, caminho) = await SalvarArquivoAsync(recebimento.Id, docBody.Arquivo);

                var docRequest = new MobileEnviarDocumentoRequest(
                    recebimento.Id,
                    docBody.TipoDocumentoId,
                    usuarioId,
                    docBody.Arquivo.FileName,
                    nomeArquivo,
                    caminho,
                    Path.GetExtension(docBody.Arquivo.FileName).TrimStart('.'),
                    docBody.Arquivo.ContentType,
                    docBody.Arquivo.Length,
                    docBody.OrdemExibicao);

                await recebimentoAppService.EnviarDocumentoAsync(docRequest, cancellationToken);
            }

            var detalhe = await recebimentoAppService.GetDetalheAsync(recebimento.Id, usuarioId, cancellationToken)
                ?? throw new RegraDeNegocioException("Nao foi possivel carregar o recebimento apos o cadastro.");

            return CreatedAtAction(nameof(GetDetalhe), new { id = detalhe.Id }, detalhe);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private async Task<(string nomeArquivo, string caminho)> SalvarArquivoAsync(ulong recebimentoId, IFormFile arquivo)
    {
        var storagePath = configuration["STORAGE_PATH"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        var pasta = Path.Combine(storagePath, "recebimentos", recebimentoId.ToString());

        Directory.CreateDirectory(pasta);

        var extensao = Path.GetExtension(arquivo.FileName);
        var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
        var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

        await using var stream = new FileStream(caminhoCompleto, FileMode.Create);
        await arquivo.CopyToAsync(stream);

        var caminhoRelativo = Path.Combine("recebimentos", recebimentoId.ToString(), nomeArquivo)
            .Replace('\\', '/');

        return (nomeArquivo, caminhoRelativo);
    }

    private ulong ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("codigoUsuario")
            ?? throw new InvalidOperationException("Identidade do usuario nao encontrada no token.");

        return ulong.Parse(claim);
    }
}

public sealed record MobileCriarRecebimentoBody(ulong EmpresaId, string? Observacao);

public sealed record MobileCriarRecebimentoNfeBody(ulong EmpresaId, string ChaveAcesso, string? Observacao);

public sealed record MobileIniciarConferenciaBody(string? Observacao);

public sealed record MobileFinalizarConferenciaBody(bool PossuiDivergencia, string? Observacao);

public sealed class MobileEnviarDocumentoBody
{
    public ulong TipoDocumentoId { get; set; }
    public int OrdemExibicao { get; set; }
    public IFormFile? Arquivo { get; set; }
}

public sealed class MobileCriarComDocumentosBody
{
    public ulong EmpresaId { get; set; }
    public string? Observacao { get; set; }
    public List<MobileDocumentoFormItem>? Documentos { get; set; }
}

public sealed class MobileDocumentoFormItem
{
    public ulong TipoDocumentoId { get; set; }
    public int OrdemExibicao { get; set; }
    public IFormFile? Arquivo { get; set; }
}
