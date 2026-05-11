using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Seed.Cfg;

internal static class CfgSeedData
{
    public static IEnumerable<CfgConsulta> GetAll()
    {
        yield return new CfgConsulta(
            identificador: "recebimentos",
            descricao: "Lista de recebimentos",
            campoChavePrimaria: "r.id",
            sql: "SELECT r.id, r.codigoRecebimento, r.statusRecebimento, r.empresaId, e.nomeFantasia AS empresaNome, r.usuarioEnvioId, u.nome AS usuarioEnvioNome, r.dataEnvio, r.dataAtualizacao, nf.numeroNota, b.dataVencimento FROM recebimentos r INNER JOIN empresas e ON e.id = r.empresaId INNER JOIN usuarios u ON u.id = r.usuarioEnvioId LEFT JOIN notasFiscais nf ON nf.recebimentoId = r.id AND nf.id = (SELECT MIN(id) FROM notasFiscais WHERE recebimentoId = r.id) LEFT JOIN boletos b ON b.recebimentoId = r.id AND b.id = (SELECT MIN(id) FROM boletos WHERE recebimentoId = r.id) ${WHERE} ${ORDERBY}",
            buscaIdentificador: "codigoRecebimento",
            buscaDescricao: "empresaNome",
            buscarCodigoAlternativo: "id",
            observacao: "Lista de recebimentos do sistema.");

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
