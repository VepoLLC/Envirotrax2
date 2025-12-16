
namespace Envirotrax.App.Server.Domain.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        return services;
    }
}