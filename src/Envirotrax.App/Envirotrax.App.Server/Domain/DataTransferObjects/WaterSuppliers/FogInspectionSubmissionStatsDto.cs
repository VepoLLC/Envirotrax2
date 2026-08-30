namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class FogInspectionDailyStatsDto
{
    public DateOnly Date { get; set; }
    public bool IsWeekend { get; set; }
    public int TotalInspections { get; set; }
    public int TotalPaidInspections { get; set; }
}

public class FogInspectionSubAccountStatsDto
{
    public int WaterSupplierId { get; set; }
    public string WaterSupplierName { get; set; } = "";
    public List<FogInspectionDailyStatsDto> DailyStats { get; set; } = new();
}

public class FogInspectionSubmissionStatsDto
{
    public List<FogInspectionDailyStatsDto> DailyStats { get; set; } = new();
    public List<FogInspectionSubAccountStatsDto>? SubAccountStats { get; set; }
}
