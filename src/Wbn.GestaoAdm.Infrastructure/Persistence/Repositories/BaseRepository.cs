using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Common.Entities;
using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories;

public class BaseRepository<T>(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : IBaseRepository<T> where T : BaseEntity
{
    private static readonly Type[] CollectionTypes =
    [
        typeof(List<>),
        typeof(ICollection<>),
        typeof(IEnumerable<>),
        typeof(IReadOnlyCollection<>),
        typeof(IReadOnlyList<>),
        typeof(HashSet<>)
    ];

    protected AppDbContext Context { get; } = context;
    protected DbSet<T> DbSet { get; } = context.Set<T>();
    protected IConfiguration Configuration { get; } = configuration;
    protected IHttpContextAccessor Accessor { get; } = accessor;

    private long UsuarioLogado => RetornaIdUsuarioLogado();
    private string VersaoBancoDados => Configuration.GetValue<string>("DadosProjeto:VersaoBancoDados") ?? "1.0.0";
    private string VersaoApi => Configuration.GetValue<string>("DadosProjeto:VersaoApi") ?? "1.0.0";
    private string VersaoFrontEnd => Configuration.GetValue<string>("DadosProjeto:VersaoFrontEnd") ?? "1.0.0";

    public virtual async Task<T> Create(T obj, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await DbSet.AddAsync(obj, cancellationToken);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente(), cancellationToken);
                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return obj;
    }

    public virtual async Task<List<T>> CreateRange(List<T> obj, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                await DbSet.AddRangeAsync(obj, cancellationToken);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente(), cancellationToken);
                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return obj;
    }

    public virtual async Task<List<T>> UpdateRange(List<T> obj, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                DbSet.UpdateRange(obj);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente(), cancellationToken);
                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return obj;
    }

    public virtual async Task<T> Update(T obj, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                Context.Entry(obj).State = EntityState.Modified;
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente(), cancellationToken);

                foreach (var property in GetEntityCollectionProperties(obj.GetType()))
                {
                    if (property.GetValue(obj) is not System.Collections.IEnumerable items)
                    {
                        continue;
                    }

                    foreach (var item in items)
                    {
                        if (item is not BaseEntity entity)
                        {
                            continue;
                        }

                        if (entity.Deletado)
                        {
                            Context.Remove(entity);
                            continue;
                        }

                        if (entity.Id == 0)
                        {
                            Context.Add(entity);
                            continue;
                        }

                        Context.Entry(entity).State = EntityState.Modified;
                    }
                }

                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });

        return obj;
    }

    public virtual async Task Remove(ulong id, CancellationToken cancellationToken = default)
    {
        var obj = await Get(id, cancellationToken);

        if (obj is null)
        {
            return;
        }

        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                DbSet.Remove(obj);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente(), cancellationToken);
                await Context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public virtual async Task<T?> Get(ulong id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public virtual async Task<List<T>> GetAll(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> RecordExists(ulong id, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(x => x.Id == id, cancellationToken);
    }

    protected virtual IEnumerable<System.Reflection.PropertyInfo> GetEntityCollectionProperties(Type entityType)
    {
        return entityType
            .GetProperties()
            .Where(property =>
                property.PropertyType.IsGenericType
                && CollectionTypes.Contains(property.PropertyType.GetGenericTypeDefinition())
                && typeof(BaseEntity).IsAssignableFrom(property.PropertyType.GetGenericArguments()[0]));
    }

    protected virtual long RetornaIdUsuarioLogado()
    {
        var claimsPrincipal = Accessor.HttpContext?.User;

        if (claimsPrincipal is null || claimsPrincipal.Identity?.IsAuthenticated != true)
        {
            return 1;
        }

        var possibleClaims = new[]
        {
            "codigoUsuario",
            ClaimTypes.NameIdentifier,
            "sub"
        };

        foreach (var claimType in possibleClaims)
        {
            var claimValue = claimsPrincipal.FindFirstValue(claimType);

            if (long.TryParse(claimValue, out var codigoUsuario))
            {
                return codigoUsuario;
            }
        }

        return 1;
    }

    protected string RetornaVariaveisAmbiente()
    {
        return RetornaVariaveisAmbiente(UsuarioLogado);
    }

    protected string RetornaVariaveisAmbienteRobo()
    {
        return RetornaVariaveisAmbiente(-3);
    }

    protected string RetornaVariaveisAmbienteServicoApi()
    {
        return RetornaVariaveisAmbiente(-1);
    }

    protected string RetornaVariaveisAmbiente(long codigoUsuario)
    {
        var sql = new StringBuilder();
        sql.Append("SET @CodigoUsuario = ");
        sql.Append(codigoUsuario);
        sql.Append(", ");
        sql.Append("@VersaoBancoDados = \"");
        sql.Append(VersaoBancoDados.Replace("\"", "\\\""));
        sql.Append("\", ");
        sql.Append("@VersaoApi = \"");
        sql.Append(VersaoApi.Replace("\"", "\\\""));
        sql.Append("\", ");
        sql.Append("@VersaoFrontEnd = \"");
        sql.Append(VersaoFrontEnd.Replace("\"", "\\\""));
        sql.Append("\";");
        return sql.ToString();
    }
}
