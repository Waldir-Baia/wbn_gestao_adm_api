using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;

public sealed class NotaFiscalRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<NotaFiscal>(context, configuration, accessor), INotaFiscalRepository
{
    public async Task<NotaFiscal?> GetByChaveAcessoAsync(
        string chaveAcesso,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chaveAcesso);

        var chaveNormalizada = chaveAcesso.Trim();

        return await DbSet.AsNoTracking()
            .Include(notaFiscal => notaFiscal.Recebimento)
            .FirstOrDefaultAsync(notaFiscal => notaFiscal.ChaveAcesso == chaveNormalizada, cancellationToken);
    }
}
