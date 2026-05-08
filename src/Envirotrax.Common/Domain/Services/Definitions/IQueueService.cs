using Azure.Storage.Queues.Models;
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
    Task SendMessageAsync(string queueName, string message, CancellationToken cancellationToken = default);

    IQueueMessageReceiver CreateMessageReceiver(string queueName);
}

public interface IQueueMessageReceiver
{
    Task<QueueMessage[]> ReceiveMessagesAsync(int? maxMessages, TimeSpan? visibilityTimeout, CancellationToken cancellationToken);
    Task DeleteMessageAsync(string messageId, string popReceipt, CancellationToken cancellationToken);
}