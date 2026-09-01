namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class FogTripTicketDailyStatsDto
{
    public DateOnly Date { get; set; }
    public bool IsWeekend { get; set; }
    public int TotalTripTickets { get; set; }
    public int TotalPaidTripTickets { get; set; }
}

public class FogTripTicketSubAccountStatsDto
{
    public int WaterSupplierId { get; set; }
    public string WaterSupplierName { get; set; } = "";
    public List<FogTripTicketDailyStatsDto> DailyStats { get; set; } = new();
}

public class FogTripTicketSubmissionStatsDto
{
    public List<FogTripTicketDailyStatsDto> DailyStats { get; set; } = new();
    public List<FogTripTicketSubAccountStatsDto>? SubAccountStats { get; set; }
}
