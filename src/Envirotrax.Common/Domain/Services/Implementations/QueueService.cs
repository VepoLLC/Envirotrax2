using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Storage.Queues;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class QueueService : QueueService<QueueOptions>, IQueueService
{
    public QueueService(IOptions<QueueOptions> options)
        : base(options)
    {
    }

}

public class QueueService<TOptions> : IQueueService<TOptions>
    where TOptions : QueueOptions
{
    private readonly QueueServiceClient _queueServiceClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public QueueService(IOptions<TOptions> options)
    {
        var uri = new Uri($"https://{options.Value.AccountName}.queue.core.windows.net");
        _queueServiceClient = new QueueServiceClient(uri, new DefaultAzureCredential());
    }

    public async Task EnsureQueueExistsAsync(string queueName, CancellationToken cancellationToken = default)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);
        await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
    }

    public Task SendMessageAsync<T>(string queueName, T message, CancellationToken cancellationToken = default)
        => SendMessageAsync(queueName, message, null, null, cancellationToken);

    public async Task SendMessageAsync<T>(string queueName, T message, TimeSpan? visibilityTimeout, TimeSpan? timeToLive, CancellationToken cancellationToken = default)
    {
        var queueClient = _queueServiceClient.GetQueueClient(queueName);

        var json = JsonSerializer.Serialize(message, _jsonOptions);

        await queueClient.SendMessageAsync(json, visibilityTimeout, timeToLive, cancellationToken);
    }
}