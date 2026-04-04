using Microsoft.Extensions.DependencyInjection;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Interfaces;
using Wbn.GestaoAdm.Application.Modules.Usuarios.Services;

namespace Wbn.GestaoAdm.Application.DependencyInjection;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUsuarioAppService, UsuarioAppService>();

        return services;
    }
}
