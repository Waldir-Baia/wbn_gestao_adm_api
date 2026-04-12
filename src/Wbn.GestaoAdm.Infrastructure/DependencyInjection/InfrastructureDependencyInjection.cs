using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Cfg;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Empresas;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Perfis;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.TiposDocumento;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Usuarios;

namespace Wbn.GestaoAdm.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A connection string 'DefaultConnection' nao foi configurada.");

        services.AddHttpContextAccessor();
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<ICfgConsultaRepository, CfgConsultaRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IPerfilRepository, PerfilRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITipoDocumentoRepository, TipoDocumentoRepository>();
        services.AddScoped<IRecebimentoRepository, RecebimentoRepository>();
        services.AddScoped<IArquivoRepository, ArquivoRepository>();
        services.AddScoped<IRecebimentoConferenciaRepository, RecebimentoConferenciaRepository>();
        services.AddScoped<IRecebimentoDivergenciaRepository, RecebimentoDivergenciaRepository>();
        services.AddScoped<IRecebimentoHistoricoRepository, RecebimentoHistoricoRepository>();

        return services;
    }
}
