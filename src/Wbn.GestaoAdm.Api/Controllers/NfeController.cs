using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Nfe.Dtos;
using Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;

namespace Wbn.GestaoAdm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/nfe")]
public sealed class NfeController(INfeAppService nfeAppService) : ControllerBase
{
    [HttpPost("certificado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarCertificado(
        [FromBody] AtualizarCertificadoDigitalEmpresaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await nfeAppService.AtualizarCertificadoDigitalAsync(request, cancellationToken);
            return NoContent();
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("sincronizar")]
    [ProducesResponseType(typeof(SincronizarNfeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SincronizarNfeResponse>> Sincronizar(
        [FromBody] SincronizarNfeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await nfeAppService.SincronizarAsync(request, cancellationToken);
            return Ok(resultado);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{chaveAcesso}")]
    [ProducesResponseType(typeof(NfeDocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NfeDocumentoResponse>> GetByChaveAcesso(
        [FromRoute] string chaveAcesso,
        [FromQuery] ulong empresaId,
        CancellationToken cancellationToken)
    {
        try
        {
            var documento = await nfeAppService.GetByChaveAcessoAsync(chaveAcesso, empresaId, cancellationToken);
            return documento is null ? NotFound() : Ok(documento);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{chaveAcesso}/xml")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetXml(
        [FromRoute] string chaveAcesso,
        [FromQuery] ulong empresaId,
        CancellationToken cancellationToken)
    {
        try
        {
            var xml = await nfeAppService.GetXmlByChaveAcessoAsync(chaveAcesso, empresaId, cancellationToken);
            return Content(xml, "application/xml");
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{chaveAcesso}/produtos")]
    [ProducesResponseType(typeof(List<NfeProdutoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<NfeProdutoResponse>>> GetProdutos(
        [FromRoute] string chaveAcesso,
        [FromQuery] ulong empresaId,
        CancellationToken cancellationToken)
    {
        try
        {
            var produtos = await nfeAppService.GetProdutosByChaveAcessoAsync(chaveAcesso, empresaId, cancellationToken);
            return Ok(produtos);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("{chaveAcesso}/manifestar")]
    [ProducesResponseType(typeof(NfeDocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NfeDocumentoResponse>> Manifestar(
        [FromRoute] string chaveAcesso,
        [FromBody] ManifestarNfeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await nfeAppService.ManifestarAsync(chaveAcesso, request, cancellationToken);
            return Ok(resultado);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
