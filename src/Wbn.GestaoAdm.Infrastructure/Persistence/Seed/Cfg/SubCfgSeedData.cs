using Wbn.GestaoAdm.Domain.Modules.Cfg.Entities;

namespace Wbn.GestaoAdm.Infrastructure.Persistence.Seed.Cfg;

internal static class SubCfgSeedData
{
    public static IEnumerable<SubCfgCampo> GetAll()
    {
        yield return new SubCfgCampo("conferenciaFila_id", "conferenciaFila", "id", "Id", "integer", 0, true, true, "r.id", null, 80);
        yield return new SubCfgCampo("conferenciaFila_codigoRecebimento", "conferenciaFila", "codigoRecebimento", "Código", "string", 1, true, true, "r.codigoRecebimento", null, 180);
        yield return new SubCfgCampo("conferenciaFila_statusRecebimento", "conferenciaFila", "statusRecebimento", "Status", "integer", 2, true, true, "r.statusRecebimento", null, 120);
        yield return new SubCfgCampo("conferenciaFila_empresaNome", "conferenciaFila", "empresaNome", "Empresa", "string", 3, true, true, "e.nomeFantasia", null, 220);
        yield return new SubCfgCampo("conferenciaFila_usuarioEnvioNome", "conferenciaFila", "usuarioEnvioNome", "Usuário envio", "string", 4, true, true, "u.nome", null, 220);
        yield return new SubCfgCampo("conferenciaFila_origem", "conferenciaFila", "origem", "Origem", "string", 5, true, true, "r.origem", null, 120);
        yield return new SubCfgCampo("conferenciaFila_dataEnvio", "conferenciaFila", "dataEnvio", "Data envio", "dateTime", 6, true, true, "r.dataEnvio", null, 160);
        yield return new SubCfgCampo("conferenciaFila_dataAtualizacao", "conferenciaFila", "dataAtualizacao", "Data atualização", "dateTime", 7, true, true, "r.dataAtualizacao", null, 160);
    }
}
