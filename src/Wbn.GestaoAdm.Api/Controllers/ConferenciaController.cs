using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Dtos;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class ConferenciaController(IConferenciaAppService conferenciaAppService) : ControllerBase
{
    [HttpPost("fila")]
    [ProducesResponseType(typeof(CfgResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CfgResultDto>> GetFila(
        [FromBody] ConferenciaFilaRequest request,
        CancellationToken cancellationToken)
    {
        var fila = await conferenciaAppService.GetFilaAsync(request, cancellationToken);
        return Ok(fila);
    }

    [HttpGet("{recebimentoId:long}")]
    [ProducesResponseType(typeof(ConferenciaDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConferenciaDetalheResponse>> GetDetalhe(ulong recebimentoId, CancellationToken cancellationToken)
    {
        var detalhe = await conferenciaAppService.GetDetalheAsync(recebimentoId, cancellationToken);
        return detalhe is null ? NotFound() : Ok(detalhe);
    }

    [HttpPut("{id:long}/iniciar")]
    [ProducesResponseType(typeof(ConferenciaDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConferenciaDetalheResponse>> Iniciar(
        ulong id,
        [FromBody] IniciarConferenciaRequest request,
        CancellationToken cancellationToken)
    {
        var detalhe = await conferenciaAppService.IniciarAsync(id, request, cancellationToken);
        return detalhe is null ? NotFound() : Ok(detalhe);
    }

    [HttpPost("{id:long}/finalizar")]
    [ProducesResponseType(typeof(ConferenciaDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConferenciaDetalheResponse>> Finalizar(
        ulong id,
        [FromBody] FinalizarConferenciaRequest request,
        CancellationToken cancellationToken)
    {
        var detalhe = await conferenciaAppService.FinalizarAsync(id, request, cancellationToken);
        return detalhe is null ? NotFound() : Ok(detalhe);
    }
}
