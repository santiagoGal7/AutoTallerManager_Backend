using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoTallerManager.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar todos los validadores de FluentValidation del ensamblado actual
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
