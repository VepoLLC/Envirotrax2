
using Envirotrax.Common.Configuration;
using Envirotrax.TaskRunner.Domain.DataTransferObjects;
using Envirotrax.TaskRunner.Domain.Services.Definitions;
using Envirotrax.TaskRunner.Workers.Sites;

namespace Envirotrax.TaskRunner.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddTaskRunnerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInternalApi(configuration.GetSection("EnvirotraxApi"))
            .AddQueueService(configuration.GetSection("Queue"));

        services.Configure<GeocodingOptions>(configuration.GetSection("Tasks:Geocoding"));
        services.AddQueueWorker(new QueueWorkerOptions<SiteGeocoder, SiteDto>(QueueNames.Sites.Geocode));

        return services;
    }

    private static IServiceCollection AddQueueWorker<TWorker, TMessage>(this IServiceCollection services, QueueWorkerOptions<TWorker, TMessage> options)
        where TWorker : notnull, IQueueWorker<TMessage>
    {
        return services
            .AddSingleton(options)
            .AddHostedService<QueueWorkerBase<TWorker, TMessage>>();
    }
}