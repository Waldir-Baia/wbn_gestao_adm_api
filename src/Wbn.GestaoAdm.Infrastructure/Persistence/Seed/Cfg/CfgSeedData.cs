using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Seed.Cfg;

internal static class CfgSeedData
{
    public static IEnumerable<CfgConsulta> GetAll()
    {
        yield return new CfgConsulta(
            identificador: "conferenciaFila",
            descricao: "Fila de conferência",
            campoChavePrimaria: "r.id",
            sql: "SELECT r.id, r.codigoRecebimento, r.statusRecebimento, e.nomeFantasia AS empresaNome, u.nome AS usuarioEnvioNome, r.origem, r.dataEnvio, r.dataAtualizacao FROM recebimentos r INNER JOIN empresas e ON e.id = r.empresaId INNER JOIN usuarios u ON u.id = r.usuarioEnvioId WHERE r.statusRecebimento IN (1, 2) ${WHEREOUT} ${ORDERBY}",
            buscaIdentificador: "codigoRecebimento",
            buscaDescricao: "empresaNome",
            buscarCodigoAlternativo: "id",
            observacao: "Fila operacional da conferência de recebimentos.");
    }
}
