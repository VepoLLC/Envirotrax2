namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class SiteRecordCountsDto
{
    public int SiteLogCount { get; set; }
    public int CsiInspectionCount { get; set; }
    public int BackflowTestCount { get; set; }
    public int OutOfServiceCount { get; set; }
    public int FogTripTicketCount { get; set; }
    public int FogInspectionCount { get; set; }
}
