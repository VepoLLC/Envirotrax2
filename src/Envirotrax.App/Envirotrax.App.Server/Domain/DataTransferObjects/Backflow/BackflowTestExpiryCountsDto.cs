
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestExpiryCountsDto
{
    public int Expired { get; set; }
    public int ThisMonth { get; set; }
    public int NextMonth { get; set; }
    public int TwoMonths { get; set; }
}
