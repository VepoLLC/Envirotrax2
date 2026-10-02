
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public enum InsuranceCheckResult
{
    NotRequired,
    NotFound,
    Unverified,
    Expired,
    InvalidCoverage,
    Valid
}

public class InsuranceCheckDto
{
    public InsuranceCheckResult Result { get; set; }

    public decimal RequiredAmount { get; set; }

    public string? Message { get; set; }

    public bool IsSatisfied => Result is InsuranceCheckResult.Valid or InsuranceCheckResult.NotRequired;
}
