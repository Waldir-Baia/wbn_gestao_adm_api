using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Dtos;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class DivergenciasController(IDivergenciaAppService divergenciaAppService) : ControllerBase
{
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DivergenciaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DivergenciaResponse>> GetById(ulong id, CancellationToken cancellationToken)
    {
        var divergencia = await divergenciaAppService.GetByIdAsync(id, cancellationToken);
        return divergencia is null ? NotFound() : Ok(divergencia);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DivergenciaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<DivergenciaResponse>> Create(
        [FromBody] CreateDivergenciaRequest request,
        CancellationToken cancellationToken)
    {
        var divergencia = await divergenciaAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = divergencia.Id }, divergencia);
    }

    [HttpPut("{id:long}/status")]
    [ProducesResponseType(typeof(DivergenciaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DivergenciaResponse>> UpdateStatus(
        ulong id,
        [FromBody] UpdateDivergenciaStatusRequest request,
        CancellationToken cancellationToken)
    {
        var divergencia = await divergenciaAppService.UpdateStatusAsync(id, request, cancellationToken);
        return divergencia is null ? NotFound() : Ok(divergencia);
    }

    [HttpPut("{id:long}/resolver")]
    [ProducesResponseType(typeof(DivergenciaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DivergenciaResponse>> Resolver(
        ulong id,
        [FromBody] ResolverDivergenciaRequest request,
        CancellationToken cancellationToken)
    {
        var divergencia = await divergenciaAppService.ResolverAsync(id, request, cancellationToken);
        return divergencia is null ? NotFound() : Ok(divergencia);
    }
}
