using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Entities;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;

public sealed class RecebimentoRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<Recebimento>(context, configuration, accessor), IRecebimentoRepository
{
    public async Task<Recebimento?> GetByCodigoRecebimentoAsync(
        string codigoRecebimento,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigoRecebimento);

        var normalizedCodigoRecebimento = codigoRecebimento.Trim();

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(
                recebimento => recebimento.CodigoRecebimento == normalizedCodigoRecebimento,
                cancellationToken);
    }

    public async Task<Recebimento?> GetDetailsByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(recebimento => recebimento.Historicos)
            .Include(recebimento => recebimento.Arquivos)
                .ThenInclude(arquivo => arquivo.TipoDocumento)
            .Include(recebimento => recebimento.NotasFiscais)
            .Include(recebimento => recebimento.Boletos)
            .Include(recebimento => recebimento.Conferencias)
            .FirstOrDefaultAsync(recebimento => recebimento.Id == id, cancellationToken);
    }

    public async Task<List<Recebimento>> GetByUsuarioEnvioIdAsync(
        ulong usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking()
            .Include(recebimento => recebimento.Empresa)
            .Include(recebimento => recebimento.Arquivos)
            .Where(recebimento => recebimento.UsuarioEnvioId == usuarioId)
            .OrderByDescending(recebimento => recebimento.DataEnvio)
            .ToListAsync(cancellationToken);
    }
}
