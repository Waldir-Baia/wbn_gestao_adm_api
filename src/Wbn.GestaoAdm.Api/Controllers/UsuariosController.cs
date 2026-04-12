using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Dtos;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class UsuariosController(IUsuarioAppService usuarioAppService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UsuarioResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var usuarios = await usuarioAppService.GetAllAsync(cancellationToken);
        return Ok(usuarios);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> GetById(ulong id, CancellationToken cancellationToken)
    {
        var usuario = await usuarioAppService.GetByIdAsync(id, cancellationToken);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UsuarioResponse>> Create(
        [FromBody] CreateUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioAppService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> Update(
        ulong id,
        [FromBody] UpdateUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioAppService.UpdateAsync(id, request, cancellationToken);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(ulong id, CancellationToken cancellationToken)
    {
        var deleted = await usuarioAppService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
