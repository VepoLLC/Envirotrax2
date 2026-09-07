
namespace Envirotrax.App.Server.Domain.Services.Definitions.Payments;

public interface IAuthorizeNetPaymentService
{
    Task<AuthorizeNetChargeResult> ChargeAsync(string dataDescriptor, string dataValue, decimal amount, AuthorizeNetBillingInfo billingInfo, CancellationToken cancellationToken);
}

public class AuthorizeNetChargeResult
{
    public bool IsApproved { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AuthorizeNetBillingInfo
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
}
