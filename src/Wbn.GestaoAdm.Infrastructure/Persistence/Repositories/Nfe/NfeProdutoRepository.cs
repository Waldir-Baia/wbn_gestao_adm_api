using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Entities;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Nfe;

public sealed class NfeProdutoRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<NfeProduto>(context, configuration, accessor), INfeProdutoRepository
{
    public async Task<List<NfeProduto>> GetByNfeDocumentoIdAsync(
        ulong nfeDocumentoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(p => p.NfeDocumentoId == nfeDocumentoId)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }
}
