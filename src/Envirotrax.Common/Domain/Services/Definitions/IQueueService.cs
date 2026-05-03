using Envirotrax.Common.Configuration;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IQueueService : IQueueService<QueueOptions>
{

}

public interface IQueueService<TOptions>
    where TOptions : QueueOptions
{
    Task EnsureQueueExistsAsync(string queueName, CancellationToken cancellationToken = default);

    Task SendMessageAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
    Task SendMessageAsync<T>(string queueName, T message, TimeSpan? visibilityTimeout, TimeSpan? timeToLive, CancellationToken cancellationToken = default);
}