using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Entities;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Perfis;

public sealed class PerfilRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<Perfil>(context, configuration, accessor), IPerfilRepository
{
    public async Task<Perfil?> GetByNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);

        var normalizedName = nome.Trim();

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(perfil => perfil.Nome == normalizedName, cancellationToken);
    }
}
