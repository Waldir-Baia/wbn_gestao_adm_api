using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Entities;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Usuarios;

public sealed class UsuarioRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<Usuario>(context, configuration, accessor), IUsuarioRepository
{
    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Email == normalizedEmail, cancellationToken);
    }

    public async Task<Usuario?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);

        var normalizedLogin = login.Trim().ToLowerInvariant();

        return await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(usuario => usuario.Login == normalizedLogin, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailForAuthenticationAsync(string email, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await DbSet
            .Include(usuario => usuario.UsuariosEmpresas)
            .FirstOrDefaultAsync(usuario => usuario.Email == normalizedEmail, cancellationToken);
    }

    public async Task<Usuario?> GetByIdForAuthenticationAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(usuario => usuario.UsuariosEmpresas)
            .FirstOrDefaultAsync(usuario => usuario.Id == id, cancellationToken);
    }
}
