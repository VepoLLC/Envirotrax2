
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// FOG inspector search criteria that cannot ride the generic <c>Query</c> pipeline, because each one matches
/// a child collection (licences, insurances) or a person inside the company rather than a column on the
/// company row. Bound with <c>[FromQuery]</c>, so these stay ordinary query-string parameters.
/// </summary>
public class FogInspectorSearchDto
{
    public string? InspectorLicenseNumber { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    /// <summary>
    /// In V2 the username IS the email, so there is no separate user id. Matches the company email or the
    /// email of any of the company's FOG inspector users.
    /// </summary>
    public string? UserEmail { get; set; }

    public string? ContactName { get; set; }

    /// <summary>
    /// Matches the company cell number (Professional.CellPhoneNumber, edited on the company profile) or the
    /// account phone of any of the company's FOG inspector users (where migrated V1 cell numbers live).
    /// </summary>
    public string? CellNumber { get; set; }
}
