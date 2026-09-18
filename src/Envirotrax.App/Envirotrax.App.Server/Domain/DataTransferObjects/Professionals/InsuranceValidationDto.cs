namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

/// <summary>
/// Mirrors V1's WaterSupplierLicensing.SaveLicenseResult for insurance policies. Deliberately a
/// separate type from ExpirationType: that one describes how close a date is to expiring, and is
/// rendered on ~20 badge grids across two client apps. This one answers "can this professional
/// submit work", which is a different question - InsufficientCoverage is not an expiration state at
/// all, and NotFound/NotRequired have no ExpirationType equivalent either.
///
/// NotFound is deliberately the default (0) value: the bug this type replaces was an unvalidated
/// policy silently defaulting to "valid" because a nullable DateTime comparison fell through.
/// Defaulting here to the most restrictive state instead means an uninitialized value fails closed.
/// </summary>
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

    /// <summary>The coverage the water supplier demands for this professional type; 0 when not required.</summary>
    public decimal RequiredCoverage { get; set; }
}
