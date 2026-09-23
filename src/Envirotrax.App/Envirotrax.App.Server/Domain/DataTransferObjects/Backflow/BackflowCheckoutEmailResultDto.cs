namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowCheckoutEmailResultDto
{
    public string Description { get; set; } = null!;

    public bool IsSent { get; set; }
}
