using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;

public sealed class ArquivoRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<Arquivo>(context, configuration, accessor), IArquivoRepository
{
    public async Task<Arquivo?> GetDetailsByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(arquivo => arquivo.TipoDocumento)
            .FirstOrDefaultAsync(arquivo => arquivo.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Arquivo>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(arquivo => arquivo.TipoDocumento)
            .Where(arquivo => arquivo.RecebimentoId == recebimentoId)
            .OrderBy(arquivo => arquivo.OrdemExibicao)
            .ThenBy(arquivo => arquivo.DataUpload)
            .ToListAsync(cancellationToken);
    }
}
