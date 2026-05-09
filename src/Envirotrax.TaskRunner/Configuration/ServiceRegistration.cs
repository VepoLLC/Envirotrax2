
using Envirotrax.Common.Configuration;
using Envirotrax.TaskRunner.Authentication;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;
using Envirotrax.TaskRunner.Domain.Services.Implementations;
using Envirotrax.TaskRunner.Workers.Sites;

namespace Envirotrax.TaskRunner.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddTaskRunnerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInternalApi(configuration.GetSection("EnvirotraxApi"))
            .AddQueueService(configuration.GetSection("Queue"));

        services.AddTransient<IKeyHashingService, KeyHashingService>();

        services
             .AddAuthentication("ApiKey")
             .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", options =>
             {
                 options.HashedApiKey = configuration["ApiKeyAuthentication:ApiKey"]
                     ?? throw new InvalidOperationException("ApiKeyAuthentication:ApiKey is not configured.");
             });

        services.AddAuthorization();

        services.Configure<GeocodingOptions>(configuration.GetSection("Tasks:Geocoding"));
        services.AddQueueWorker(new QueueWorkerOptions<SiteGeocoder, SiteDto>(QueueNames.Sites.Geocode)
        {
            MaxDequeuCount = 2
        });

        return services;
    }

    private static IServiceCollection AddQueueWorker<TWorker, TMessage>(this IServiceCollection services, QueueWorkerOptions<TWorker, TMessage> options)
        where TWorker : class, IQueueWorker<TMessage>
    {
        return services
            .AddSingleton(options)
            .AddTransient<TWorker>()
            .AddHostedService<QueueWorkerBase<TWorker, TMessage>>();
    }
}