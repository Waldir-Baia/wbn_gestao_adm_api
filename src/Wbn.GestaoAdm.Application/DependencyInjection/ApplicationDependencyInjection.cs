using Microsoft.Extensions.DependencyInjection;
using Wbn.GestaoAdm.Application.Modules.Cfg.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Cfg.Services;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Conferencia.Services;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Divergencias.Services;
using Wbn.GestaoAdm.Application.Modules.Documentos.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Documentos.Services;
using Wbn.GestaoAdm.Application.Modules.Empresas.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Empresas.Services;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Recebimentos.Services;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Services;

namespace Wbn.GestaoAdm.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICfgConsultaAppService, CfgConsultaAppService>();
        services.AddScoped<IConferenciaAppService, ConferenciaAppService>();
        services.AddScoped<IDivergenciaAppService, DivergenciaAppService>();
        services.AddScoped<IDocumentoAppService, DocumentoAppService>();
        services.AddScoped<IEmpresaAppService, EmpresaAppService>();
        services.AddScoped<IUsuarioAppService, UsuarioAppService>();
        services.AddScoped<IRecebimentoAppService, RecebimentoAppService>();

        return services;
    }
}
