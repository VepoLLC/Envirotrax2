
using Envirotrax.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Envirotrax.TaskRunner.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddTaskRunnerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInternalApi(configuration.GetSection("EnvirotraxApi"))
            .AddQueueService(configuration.GetSection("Queue"));

        services.Configure<GeocodingOptions>(configuration.GetSection("Tasks:Geocoding"));

        return services;
    }
}