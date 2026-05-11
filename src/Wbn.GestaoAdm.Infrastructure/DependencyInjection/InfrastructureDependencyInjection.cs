using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wbn.GestaoAdm.Application.Modules.Nfe.Interfaces;
using Wbn.GestaoAdm.Domain.Modules.Cfg.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Empresas.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Nfe.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Perfis.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Recebimentos.Repositories;
using Wbn.GestaoAdm.Domain.Modules.TiposDocumento.Repositories;
using Wbn.GestaoAdm.Domain.Modules.Usuarios.Repositories;
using Wbn.GestaoAdm.Infrastructure.Persistence.Contexts;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Cfg;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Empresas;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Nfe;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Perfis;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Recebimentos;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.TiposDocumento;
using Wbn.GestaoAdm.Infrastructure.Persistence.Repositories.Usuarios;
using Wbn.GestaoAdm.Infrastructure.Sefaz;

namespace Wbn.GestaoAdm.Infrastructure.DependencyInjection;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var dbServer   = configuration["DB_SERVER"]   ?? throw new InvalidOperationException("A variavel DB_SERVER nao foi configurada.");
        var dbPort     = configuration["DB_PORT"]     ?? "3306";
        var dbName     = configuration["DB_NAME"]     ?? throw new InvalidOperationException("A variavel DB_NAME nao foi configurada.");
        var dbUser     = configuration["DB_USER"]     ?? throw new InvalidOperationException("A variavel DB_USER nao foi configurada.");
        var dbPassword = configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("A variavel DB_PASSWORD nao foi configurada.");

        var connectionString = $"Server={dbServer};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};Allow User Variables=true;";

        services.AddHttpContextAccessor();
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

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
        services.AddScoped<INfeDocumentoRepository, NfeDocumentoRepository>();
        services.AddScoped<INfeProdutoRepository, NfeProdutoRepository>();
        services.AddScoped<ISefazNfeClient, SefazNfeClient>();
        services.AddScoped<ICertificadoDigitalProvider, CertificadoDigitalProvider>();

        return services;
    }
}
