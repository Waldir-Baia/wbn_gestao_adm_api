using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Entities;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.TiposDocumento;

public sealed class TipoDocumentoRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<TipoDocumento>(context, configuration, accessor), ITipoDocumentoRepository
{
    public async Task<TipoDocumento?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        var normalizedNome = nome.Trim();

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(tipoDocumento => tipoDocumento.Nome == normalizedNome, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TipoDocumento>> GetAllAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Where(tipoDocumento => tipoDocumento.Ativo)
            .ToListAsync(cancellationToken);
    }
}
