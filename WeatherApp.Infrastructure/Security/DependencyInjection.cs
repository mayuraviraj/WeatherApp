using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp.Infrastructure.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddSecurityInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITokenService, TokenService>();
        return services;
    }
}