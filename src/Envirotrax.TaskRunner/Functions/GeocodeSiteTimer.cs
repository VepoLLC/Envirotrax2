using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Envirotrax.TaskRunner.Functions;

public class GeocodeSiteTimer
{
    private readonly ILogger _logger;

    public GeocodeSiteTimer(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GeocodeSiteTimer>();
    }

    [Function("GeocodeSiteTimer")]
    public void Run([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
        
        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}