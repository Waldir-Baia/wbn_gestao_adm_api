using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Documentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Documentos.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DocumentosController(IDocumentoAppService documentoAppService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CriarDocumentoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CriarDocumentoResponse>> Create(
        [FromBody] CriarDocumentoRequest request,
        CancellationToken cancellationToken)
    {
        var documento = await documentoAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = documento.Id }, documento);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentoResponse>> GetById(ulong id, CancellationToken cancellationToken)
    {
        var documento = await documentoAppService.GetByIdAsync(id, cancellationToken);
        return documento is null ? NotFound() : Ok(documento);
    }

    [HttpGet("recebimento/{recebimentoId:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<DocumentoResponse>>> GetByRecebimentoId(
        ulong recebimentoId,
        CancellationToken cancellationToken)
    {
        var documentos = await documentoAppService.GetByRecebimentoIdAsync(recebimentoId, cancellationToken);
        return Ok(documentos);
    }

    [HttpPut("{id:long}/inativar")]
    [ProducesResponseType(typeof(DocumentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DocumentoResponse>> Inativar(
        ulong id,
        [FromBody] InativarDocumentoRequest? request,
        CancellationToken cancellationToken)
    {
        var documento = await documentoAppService.InativarAsync(id, request ?? new InativarDocumentoRequest(null), cancellationToken);
        return documento is null ? NotFound() : Ok(documento);
    }

    [HttpGet("tipos-documento")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TipoDocumentoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<TipoDocumentoResponse>>> GetTiposDocumento(CancellationToken cancellationToken)
    {
        var tiposDocumento = await documentoAppService.GetTiposDocumentoAtivosAsync(cancellationToken);
        return Ok(tiposDocumento);
    }
}
