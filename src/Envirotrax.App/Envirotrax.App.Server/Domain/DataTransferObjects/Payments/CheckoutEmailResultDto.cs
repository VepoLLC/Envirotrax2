namespace Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

public class CheckoutEmailResultDto
{
    public string Description { get; set; } = null!;

    public bool IsSent { get; set; }
}
