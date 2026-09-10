
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

/// <summary>
/// One row of the dashboard's combined License &amp; Insurance grid. Licenses and insurance policies
/// stay in their own tables; only this read model merges them, via a SQL UNION.
/// </summary>
public class ProfessionalDashboardLicenseInsuranceDto
{
    public int Id { get; set; }

    public ProfessionalDashboardRowType RowType { get; set; }

    /// <summary>License type name, or "Insurance Policy" for insurance rows.</summary>
    public string TypeName { get; set; } = null!;

    public string Number { get; set; } = null!;

    /// <summary>Assigned user for licenses; the owning company for insurance policies.</summary>
    public string? AssignedTo { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public ExpirationType ExpirationType { get; set; }
}

public enum ProfessionalDashboardRowType
{
    License,
    Insurance
}
