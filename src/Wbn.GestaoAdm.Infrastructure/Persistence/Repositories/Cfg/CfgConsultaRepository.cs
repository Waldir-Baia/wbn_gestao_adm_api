using System.Data;
using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Cfg;

public sealed class CfgConsultaRepository(
    AppDbContext context,
    IConfiguration configuration,
    IHttpContextAccessor accessor) : BaseRepository<CfgConsulta>(context, configuration, accessor), ICfgConsultaRepository
{
    public async Task<CfgConsulta?> GetByIdentificadorAsync(string identificador, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identificador);

        var identificadorNormalizado = identificador.Trim();

        return await DbSet.AsNoTracking()
            .Include(cfg => cfg.SubCfgCampos)
            .FirstOrDefaultAsync(cfg => cfg.Identificador == identificadorNormalizado, cancellationToken);
    }

    public async Task<DataTable> ExecuteQueryAsync(
        string sql,
        IEnumerable<DbParameter> parameters,
        CancellationToken cancellationToken = default)
    {
        var dataTable = new DataTable();

        await using var connection = Context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = RetornaVariaveisAmbiente();
        await command.ExecuteNonQueryAsync(cancellationToken);

        command.Parameters.Clear();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        dataTable.Load(reader);

        return dataTable;
    }
}
