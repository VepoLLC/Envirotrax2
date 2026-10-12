
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

public enum LicenseCheckResult
{
    NotRequired,
    NotFound,
    Unverified,
    Expired,
    Valid
}

public class LicenseCheckDto
{
    public LicenseCheckResult Result { get; set; }

    public string? Label { get; set; }

    public string? Message { get; set; }

    public string? LicenseNumber { get; set; }

    public string? LicenseTypeName { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool IsSatisfied => Result is LicenseCheckResult.Valid or LicenseCheckResult.NotRequired;
}

public class BpatLicenseCheckDto
{
    public LicenseCheckDto License { get; set; } = null!;

    public LicenseCheckDto FireLicense { get; set; } = null!;
}
