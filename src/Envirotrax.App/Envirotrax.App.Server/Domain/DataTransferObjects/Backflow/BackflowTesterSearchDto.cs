
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

/// <summary>
/// Backflow tester search criteria that cannot ride the generic <c>Query</c> pipeline, because each one matches
/// a child collection (licences, insurances) or a person inside the company rather than a column on the
/// company row. Bound with <c>[FromQuery]</c>, so these stay ordinary query-string parameters.
/// </summary>
public class BackflowTesterSearchDto
{
    public string? BpatLicenseNumber { get; set; }

    public string? FireLicenseNumber { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    /// <summary>
    /// In V2 the username IS the email, so there is no separate user id. Matches the company email or the
    /// email of any of the company's backflow-tester users.
    /// </summary>
    public string? UserEmail { get; set; }

    public string? ContactName { get; set; }

    public string? CellNumber { get; set; }
}
