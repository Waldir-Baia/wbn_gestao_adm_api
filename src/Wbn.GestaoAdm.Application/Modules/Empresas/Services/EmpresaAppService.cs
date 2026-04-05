using Wbn.GestaoAdm.Application.Modules.Empresas.Dtos;
using Wbn.GestaoAdm.Application.Modules.Empresas.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Empresas.Services;

public sealed class EmpresaAppService(IEmpresaRepository empresaRepository) : IEmpresaAppService
{
    public async Task<IReadOnlyCollection<EmpresaLookupResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var empresas = await empresaRepository.GetAll();

        return empresas
            .OrderBy(empresa => empresa.NomeFantasia)
            .Select(MapToResponse)
            .ToArray();
    }

    private static EmpresaLookupResponse MapToResponse(Empresa empresa)
    {
        return new EmpresaLookupResponse(
            empresa.Id,
            empresa.NomeFantasia,
            empresa.RazaoSocial,
            empresa.Cnpj,
            empresa.Ativo);
    }
}
