namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class BackflowDailyStatsDto
{
    public DateOnly Date { get; set; }
    public bool IsWeekend { get; set; }
    public int TotalTests { get; set; }
    public int TotalPaidTests { get; set; }
}

public class BackflowSubAccountStatsDto
{
    public string WaterSupplierName { get; set; } = "";
    public List<BackflowDailyStatsDto> DailyStats { get; set; } = new();
}

public class BackflowSubmissionStatsDto
{
    public List<BackflowDailyStatsDto> DailyStats { get; set; } = new();
    public List<BackflowSubAccountStatsDto>? SubAccountStats { get; set; }
}
