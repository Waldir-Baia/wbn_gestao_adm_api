using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;

public sealed class RecebimentoHistoricoRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<RecebimentoHistorico>(context, configuration, accessor), IRecebimentoHistoricoRepository
{
    public async Task<IReadOnlyCollection<RecebimentoHistorico>> GetByRecebimentoIdAsync(
        ulong recebimentoId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(historico => historico.RecebimentoId == recebimentoId)
            .OrderBy(historico => historico.DataCadastro)
            .ToListAsync(cancellationToken);
    }
}
