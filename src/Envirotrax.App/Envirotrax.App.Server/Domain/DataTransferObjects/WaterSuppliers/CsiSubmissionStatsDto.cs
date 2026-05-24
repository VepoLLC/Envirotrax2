namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class CsiDailyStatsDto
{
    public DateOnly Date { get; set; }
    public bool IsWeekend { get; set; }
    public int TotalInspections { get; set; }
    public int TotalPaidInspections { get; set; }
}

public class CsiSubAccountStatsDto
{
    public string WaterSupplierName { get; set; } = "";
    public List<CsiDailyStatsDto> DailyStats { get; set; } = new();
}

public class CsiSubmissionStatsDto
{
    public List<CsiDailyStatsDto> DailyStats { get; set; } = new();
    public List<CsiSubAccountStatsDto>? SubAccountStats { get; set; }
}
