using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class GeocodeSite
{
    private readonly ILogger<GeocodeSite> _logger;

    public GeocodeSite(ILogger<GeocodeSite> logger)
    {
        _logger = logger;
    }

    [Function(nameof(GeocodeSite))]
    public void Run([QueueTrigger("myqueue-items", Connection = "AzureWebJobsStorage__accountName")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    }
}