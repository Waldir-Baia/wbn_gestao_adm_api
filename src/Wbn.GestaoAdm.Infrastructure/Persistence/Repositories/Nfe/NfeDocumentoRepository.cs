using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Nfe;

public sealed class NfeDocumentoRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<NfeDocumento>(context, configuration, accessor), INfeDocumentoRepository
{
    public async Task<NfeDocumento?> GetByChaveAcessoAsync(
        string chaveAcesso,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chaveAcesso);

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(d => d.ChaveAcesso == chaveAcesso.Trim(), cancellationToken);
    }

    public async Task<NfeDocumento?> GetWithProdutosByChaveAcessoAsync(
        string chaveAcesso,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chaveAcesso);

        return await DbSet.AsNoTracking()
            .Include(d => d.Produtos)
            .FirstOrDefaultAsync(d => d.ChaveAcesso == chaveAcesso.Trim(), cancellationToken);
    }

    public async Task<List<NfeDocumento>> GetByEmpresaIdAsync(
        ulong empresaId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(d => d.EmpresaId == empresaId)
            .OrderByDescending(d => d.DataCadastro)
            .ToListAsync(cancellationToken);
    }
}
