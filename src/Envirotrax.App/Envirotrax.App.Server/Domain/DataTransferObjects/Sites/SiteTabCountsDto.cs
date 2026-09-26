
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class SiteTabCountsDto
{
    public int LogHistoryCount { get; set; }
    public int CsiCount { get; set; }
    public int BackflowCount { get; set; }
    public int OutOfServiceCount { get; set; }
    public int TripTicketCount { get; set; }
    public int FogCount { get; set; }
}
