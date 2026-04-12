using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Empresas.Dtos;
using Wbn.GestaoAdm.Application.Modules.Empresas.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class EmpresasController(IEmpresaAppService empresaAppService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EmpresaLookupResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<EmpresaLookupResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var empresas = await empresaAppService.GetAllAsync(cancellationToken);
        return Ok(empresas);
    }
}
