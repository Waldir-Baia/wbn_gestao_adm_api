using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;

public sealed class RecebimentoConferenciaRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<RecebimentoConferencia>(context, configuration, accessor), IRecebimentoConferenciaRepository
{
    public async Task<IReadOnlyCollection<RecebimentoConferencia>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(conferencia => conferencia.RecebimentoId == recebimentoId)
            .OrderByDescending(conferencia => conferencia.DataConferencia)
            .ToListAsync(cancellationToken);
    }
}
