using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Entities;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Empresas;

public sealed class EmpresaRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<Empresa>(context, configuration, accessor), IEmpresaRepository
{
    public async Task<Empresa?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cnpj);

        var normalizedCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(empresa => empresa.Cnpj == normalizedCnpj, cancellationToken);
    }
}
