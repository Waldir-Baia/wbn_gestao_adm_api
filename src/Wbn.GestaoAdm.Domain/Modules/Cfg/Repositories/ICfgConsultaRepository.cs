using System.Data;
using Wbn.GestaoAdm.Domain.Common.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;

namespace Wbn.GestaoAdm.Domain.Modules.Cfg.Repositories;

public interface ICfgConsultaRepository : IBaseRepository<CfgConsulta>
{
    Task<CfgConsulta?> GetByIdentificadorAsync(string identificador, CancellationToken cancellationToken = default);
    Task<DataTable> ExecuteQueryAsync(string sql, IEnumerable<System.Data.Common.DbParameter> parameters, CancellationToken cancellationToken = default);
}
