namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public enum InsuranceStatus
{
    NotFound = 0,
    AwaitingValidation = 1,
    Expired = 2,
    InsufficientCoverage = 3,
    Valid = 4,
    NotRequired = 5
}

public class InsuranceValidationDto
{
    public InsuranceStatus Status { get; set; }

    public decimal RequiredCoverage { get; set; }
}
