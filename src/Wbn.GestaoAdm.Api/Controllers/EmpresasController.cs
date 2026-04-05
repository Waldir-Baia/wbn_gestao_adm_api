using Microsoft.AspNetCore.Mvc;
using Wbn.GestaoAdm.Application.Modules.Empresas.Dtos;
using Wbn.GestaoAdm.Application.Modules.Empresas.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmpresasController(IEmpresaAppService empresaAppService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<EmpresaLookupResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<EmpresaLookupResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var empresas = await empresaAppService.GetAllAsync(cancellationToken);
        return Ok(empresas);
    }
}
