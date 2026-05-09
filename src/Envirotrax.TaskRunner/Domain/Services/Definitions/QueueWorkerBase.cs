
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.TaskRunner.Domain.Services.Definitions;

public class QueueWorkerBase<TWorker, TMessage> : BackgroundService
    where TWorker : notnull, IQueueWorker<TMessage>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly QueueWorkerOptions<TWorker, TMessage> _options;
    private readonly IQueueService _queuService;
    private readonly IQueueMessageReceiver _queueClient;
    private readonly ILogger<QueueWorkerBase<TWorker, TMessage>> _logger;

    public QueueWorkerBase(
        IServiceProvider serviceProvider,
        QueueWorkerOptions<TWorker, TMessage> options,
        IQueueService queueService,
        ILogger<QueueWorkerBase<TWorker, TMessage>> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _queuService = queueService;
        _logger = logger;

        _queueClient = _queuService.CreateMessageReceiver(options.QueueName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queuService.EnsureQueueExistsAsync(_options.QueueName, cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Receiving queue messages for {QueueName}", _options.QueueName);

            try
            {
                var messages = await _queueClient.ReceiveMessagesAsync(_options.MaxMessages, _options.VisibilityTimeout, stoppingToken);

                if (messages.Length == 0)
                {
                    await Task.Delay(_options.PollingInterval, stoppingToken);
                    continue;
                }

                await Task.WhenAll(messages.Select(msg => ProcessMessageAsync(msg, stoppingToken)));
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed while receiveing queue messages. Queue name: {QueueName}", _options.QueueName);
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }

    private async Task ProcessDeadLetterAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var poisonQueueName = _options.QueueName + "-poison";
            _logger.LogInformation("{QueueName} reached max dequeue count {Count}. Moving it to poison queue {PoisonQueueName}", _options.QueueName, _options.MaxDequeuCount, poisonQueueName);

            await _queuService.EnsureQueueExistsAsync(poisonQueueName);

            await _queuService.SendMessageAsync(poisonQueueName, message.MessageText, cancellationToken);
            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to move queue message to poison queue. Queue name: {QueueName}, Queue content: {Content}", _options.QueueName, message.MessageText);
        }
    }

    private async Task ProcessMessageAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var worker = _serviceProvider.GetRequiredService<TWorker>();

            var deserializedMessage = JsonSerializer.Deserialize<TMessage>(message.MessageText);

            await worker.DoWorkAsync(deserializedMessage, cancellationToken);
            await _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);

            _logger.LogInformation("Successfully processed queue {QueueName}", _options.QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {Id}. Attempt {Count}. Queue name: {QueueName}", message.MessageId, message.DequeueCount, _options.QueueName);

            if (message.DequeueCount >= _options.MaxDequeuCount)
            {
                await ProcessDeadLetterAsync(message, cancellationToken);
            }
        }
    }
}

public class QueueWorkerOptions<TWorker, TMessage>
    where TWorker : notnull, IQueueWorker<TMessage>
{
    public string QueueName { get; }
    public int MaxMessages { get; set; } = 10;
    public int MaxDequeuCount { get; set; } = 5;
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan VisibilityTimeout { get; set; } = TimeSpan.FromMinutes(5);

    public QueueWorkerOptions(string queueName)
    {
        QueueName = queueName;
    }
}

public interface IQueueWorker<TMessage>
{
    Task DoWorkAsync(TMessage? queueMessage, CancellationToken cancellationToken);
}