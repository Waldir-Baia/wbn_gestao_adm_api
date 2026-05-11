using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Empresas.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;

namespace Wbn.GestaoAdm.Api.Controllers.Mobile;

[ApiController]
[Authorize]
[Route("api/mobile/empresas")]
public sealed class MobileEmpresasController(IMobileEmpresaAppService empresaAppService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<MobileEmpresaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<MobileEmpresaResponse>>> GetEmpresas(
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioId = ObterUsuarioId();
            var empresas = await empresaAppService.GetEmpresasDoUsuarioAsync(usuarioId, cancellationToken);
            return Ok(empresas);
        }
        catch (RegraDeNegocioException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private ulong ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("codigoUsuario")
            ?? throw new InvalidOperationException("Identidade do usuario nao encontrada no token.");

        return ulong.Parse(claim);
    }
}
