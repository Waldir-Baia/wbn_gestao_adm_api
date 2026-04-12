using System.Data;
using System.Text.Json;
using MySqlConnector;
using Wbn.GestaoAdm.Application.Modules.Cfg.Dtos;
using Wbn.GestaoAdm.Application.Modules.Cfg.Interfaces;
using Wbn.GestaoAdm.Domain.Common.Exceptions;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Repositories;

namespace Wbn.GestaoAdm.Application.Modules.Cfg.Services;

public sealed class CfgConsultaAppService(ICfgConsultaRepository cfgConsultaRepository) : ICfgConsultaAppService
{
    public async Task<CfgResultDto> ProcessQueryAsync(
        string identificador,
        CfgRequestDataDto request,
        CancellationToken cancellationToken = default)
    {
        var cfg = await cfgConsultaRepository.GetByIdentificadorAsync(identificador, cancellationToken)
            ?? throw new RegraDeNegocioException($"CFG não encontrado. - {identificador}");

        if (string.IsNullOrWhiteSpace(cfg.Sql))
        {
            throw new RegraDeNegocioException($"CFG não contém SQL. - {identificador}");
        }

        var allowedFields = cfg.SubCfgCampos
            .OrderBy(campo => campo.OrdemCampo)
            .ToArray();

        var sql = cfg.Sql;
        var parameters = new List<System.Data.Common.DbParameter>();
        var parameterIndex = 0;

        var where = BuildWhereClause(request, allowedFields, parameters, ref parameterIndex);
        var orderBy = BuildOrderByClause(request, allowedFields, cfg.CampoChavePrimaria);

        sql = sql.Replace("${WHERE}", where);
        sql = sql.Replace("${WHEREOUT}", string.IsNullOrWhiteSpace(where) ? string.Empty : $" AND {where[6..]}");
        sql = sql.Replace("${ORDERBY}", string.IsNullOrWhiteSpace(orderBy) ? string.Empty : $"ORDER BY {orderBy}");
        sql = sql.Replace("${ORDERBYOUT}", string.IsNullOrWhiteSpace(orderBy) ? string.Empty : $", {orderBy}");

        var page = request.Page > 0 ? request.Page - 1 : 0;
        var pageSize = request.DataCountByPage > 0 ? request.DataCountByPage : 20;
        var offset = page * pageSize;

        sql += " LIMIT @pagina, @count";
        parameters.Add(new MySqlParameter("@pagina", MySqlDbType.Int32) { Value = offset });
        parameters.Add(new MySqlParameter("@count", MySqlDbType.Int32) { Value = pageSize });

        var dataTable = await cfgConsultaRepository.ExecuteQueryAsync(sql, parameters, cancellationToken);

        return new CfgResultDto(
            SerializeDataTable(dataTable),
            cfg.CampoChavePrimaria,
            cfg.BuscaIdentificador,
            cfg.BuscaDescricao,
            cfg.BuscarCodigoAlternativo,
            allowedFields.Select(MapField).ToArray());
    }

    private static string BuildWhereClause(
        CfgRequestDataDto request,
        IReadOnlyCollection<SubCfgCampo> allowedFields,
        List<System.Data.Common.DbParameter> parameters,
        ref int parameterIndex)
    {
        var clauses = BuildFilterClauses(request.Filter, allowedFields, parameters, ref parameterIndex);
        var codeClauses = BuildFilterClauses(request.FilterCode, allowedFields, parameters, ref parameterIndex);

        if (clauses.Any() && codeClauses.Any())
        {
            return $"WHERE ({string.Join(" AND ", clauses)}) OR ({string.Join(" AND ", codeClauses)})";
        }

        if (clauses.Any())
        {
            return $"WHERE {string.Join(" AND ", clauses)}";
        }

        if (codeClauses.Any())
        {
            return $"WHERE {string.Join(" AND ", codeClauses)}";
        }

        return string.Empty;
    }

    private static List<string> BuildFilterClauses(
        IReadOnlyCollection<CfgFilterFieldDto>? filters,
        IReadOnlyCollection<SubCfgCampo> allowedFields,
        List<System.Data.Common.DbParameter> parameters,
        ref int parameterIndex)
    {
        var clauses = new List<string>();
        if (filters is null)
        {
            return clauses;
        }

        var allowedOperators = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "=", "!=", "<>", "<", "<=", ">", ">=", "LIKE", "NOT LIKE", "IN", "NOT IN"
        };

        foreach (var filter in filters)
        {
            var field = allowedFields.FirstOrDefault(campo => campo.Campo == filter.Field && campo.PermitirFiltro);
            if (field is null)
            {
                throw new RegraDeNegocioException($"Campo de filtro não permitido: {filter.Field}");
            }

            var op = (filter.FilterType ?? string.Empty).Trim().ToUpperInvariant();
            if (!allowedOperators.Contains(op))
            {
                throw new RegraDeNegocioException($"Operador não permitido: {filter.FilterType}");
            }

            if (op is "IN" or "NOT IN")
            {
                var values = filter.FilterArray ?? [];
                if (values.Length == 0)
                {
                    throw new RegraDeNegocioException("Filtro IN sem valores.");
                }

                var paramNames = new List<string>();
                foreach (var value in values)
                {
                    var paramName = $"@p{parameterIndex++}";
                    paramNames.Add(paramName);
                    parameters.Add(new MySqlParameter(paramName, MySqlDbType.VarChar) { Value = value });
                }

                clauses.Add($"{field.CampoBusca} {op} ({string.Join(",", paramNames)})");
                continue;
            }

            var scalarParamName = $"@p{parameterIndex++}";
            parameters.Add(new MySqlParameter(scalarParamName, MySqlDbType.VarChar) { Value = filter.Filter ?? string.Empty });
            clauses.Add($"{field.CampoBusca} {op} {scalarParamName}");
        }

        return clauses;
    }

    private static string BuildOrderByClause(
        CfgRequestDataDto request,
        IReadOnlyCollection<SubCfgCampo> allowedFields,
        string primaryKey)
    {
        if (!string.IsNullOrWhiteSpace(request.OrderByField))
        {
            var field = allowedFields.FirstOrDefault(campo => campo.Campo == request.OrderByField);
            if (field is null)
            {
                throw new RegraDeNegocioException($"Campo de ordenação não permitido: {request.OrderByField}");
            }

            var orderType = string.Equals(request.OrderByType, "ASC", StringComparison.OrdinalIgnoreCase)
                ? "ASC"
                : "DESC";

            return $"{field.CampoBusca} {orderType}";
        }

        return string.IsNullOrWhiteSpace(primaryKey) ? string.Empty : $"{primaryKey} DESC";
    }

    private static string SerializeDataTable(DataTable table)
    {
        var rows = table.Rows.Cast<DataRow>()
            .Select(row => table.Columns.Cast<DataColumn>()
                .ToDictionary(column => column.ColumnName, column => row[column]))
            .ToArray();

        return JsonSerializer.Serialize(rows);
    }

    private static SubCfgCampoDto MapField(SubCfgCampo campo)
    {
        return new SubCfgCampoDto(
            campo.Campo,
            campo.Descricao,
            campo.TipoDados,
            campo.OrdemCampo,
            campo.PermitirFiltro,
            campo.Visivel,
            campo.CampoBusca,
            campo.Mascara,
            campo.LarguraColuna);
    }
}
