using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;

public sealed class RecebimentoDivergenciaRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<RecebimentoDivergencia>(context, configuration, accessor), IRecebimentoDivergenciaRepository
{
    public async Task<RecebimentoDivergencia?> GetDetailsByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(divergencia => divergencia.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<RecebimentoDivergencia>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(divergencia => divergencia.RecebimentoId == recebimentoId)
            .OrderByDescending(divergencia => divergencia.DataCadastro)
            .ToListAsync(cancellationToken);
    }
}
