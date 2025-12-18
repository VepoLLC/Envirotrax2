
using Envirotrax.App.Server.Data.Configuration;
using Envirotrax.App.Server.Domain.Configuration;

namespace Envirotrax.App.Server.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        return services
            .AddDataServices(configuration, environment)
            .AddDomainServices(configuration, environment);
    }
}