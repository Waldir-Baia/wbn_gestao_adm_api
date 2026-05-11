using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Dtos;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class RecebimentosController(IRecebimentoAppService recebimentoAppService) : ControllerBase
{
    [HttpPost("consultar")]
    [ProducesResponseType(typeof(CfgResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CfgResultDto>> GetLista(
        [FromBody] RecebimentosListaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await recebimentoAppService.GetListaAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RecebimentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecebimentoResponse>> GetById(ulong id, CancellationToken cancellationToken)
    {
        var recebimento = await recebimentoAppService.GetByIdAsync(id, cancellationToken);
        return recebimento is null ? NotFound() : Ok(recebimento);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RecebimentoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RecebimentoResponse>> Create(
        [FromBody] CreateRecebimentoRequest request,
        CancellationToken cancellationToken)
    {
        var recebimento = await recebimentoAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = recebimento.Id }, recebimento);
    }

    [HttpPut("{id:long}/status")]
    [ProducesResponseType(typeof(RecebimentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecebimentoResponse>> UpdateStatus(
        ulong id,
        [FromBody] UpdateRecebimentoStatusRequest request,
        CancellationToken cancellationToken)
    {
        var recebimento = await recebimentoAppService.UpdateStatusAsync(id, request, cancellationToken);
        return recebimento is null ? NotFound() : Ok(recebimento);
    }
}
