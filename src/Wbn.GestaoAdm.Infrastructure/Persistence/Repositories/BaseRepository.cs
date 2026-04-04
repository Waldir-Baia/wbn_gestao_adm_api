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
    protected AppDbContext Context { get; } = context;
    protected DbSet<T> DbSet { get; } = context.Set<T>();
    protected IConfiguration Configuration { get; } = configuration;
    protected IHttpContextAccessor Accessor { get; } = accessor;

    private long UsuarioLogado => RetornaIdUsuarioLogado();
    private string VersaoBancoDados => Configuration.GetValue<string>("DadosProjeto:VersaoBancoDados") ?? "1.0.0";
    private string VersaoApi => Configuration.GetValue<string>("DadosProjeto:VersaoApi") ?? "1.0.0";
    private string VersaoFrontEnd => Configuration.GetValue<string>("DadosProjeto:VersaoFrontEnd") ?? "1.0.0";

    public virtual async Task<T> Create(T obj)
    {
        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                await DbSet.AddAsync(obj);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente());
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return obj;
    }

    public virtual async Task<List<T>> CreateRange(List<T> obj)
    {
        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                await DbSet.AddRangeAsync(obj);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente());
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return obj;
    }

    public virtual async Task<List<T>> UpdateRange(List<T> obj)
    {
        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                DbSet.UpdateRange(obj);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente());
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return obj;
    }

    public virtual async Task<T> Update(T obj)
    {
        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                Context.Entry(obj).State = EntityState.Modified;
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente());

                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (!prop.PropertyType.IsGenericType)
                    {
                        continue;
                    }

                    var genericTypeDefinition = prop.PropertyType.GetGenericTypeDefinition();
                    var isCollection = genericTypeDefinition == typeof(List<>)
                        || genericTypeDefinition == typeof(ICollection<>)
                        || genericTypeDefinition == typeof(IEnumerable<>);

                    if (!isCollection)
                    {
                        continue;
                    }

                    var itemType = prop.PropertyType.GetGenericArguments()[0];

                    if (!typeof(BaseEntity).IsAssignableFrom(itemType))
                    {
                        continue;
                    }

                    if (prop.GetValue(obj) is not System.Collections.IEnumerable list)
                    {
                        continue;
                    }

                    foreach (var item in list)
                    {
                        if (item is not BaseEntity entity)
                        {
                            continue;
                        }

                        if (entity.Id == 0 && !entity.Deletado)
                        {
                            Context.Add(entity);
                        }
                        else if (entity.Id > 0 && !entity.Deletado)
                        {
                            Context.Entry(entity).State = EntityState.Modified;
                        }
                        else if (entity.Deletado)
                        {
                            Context.Remove(entity);
                        }
                    }
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });

        return obj;
    }

    public virtual async Task Remove(ulong id)
    {
        var obj = await Get(id);

        if (obj is null)
        {
            return;
        }

        var executionStrategy = Context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                DbSet.Remove(obj);
                await Context.Database.ExecuteSqlRawAsync(RetornaVariaveisAmbiente());
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public virtual async Task<T?> Get(ulong id)
    {
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public virtual async Task<List<T>> GetAll()
    {
        return await DbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<List<T>> GetAll(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task<bool> RecordExists(ulong id)
    {
        return await DbSet.AnyAsync(x => x.Id == id);
    }

    public virtual Task UpdateCampo(T obj, ulong id)
    {
        throw new NotImplementedException();
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
