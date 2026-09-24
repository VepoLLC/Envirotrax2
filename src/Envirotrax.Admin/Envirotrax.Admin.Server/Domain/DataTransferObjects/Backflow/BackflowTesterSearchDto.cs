
namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

/// <summary>
/// Mirrors the App's BackflowTesterSearchDto. Criteria that cannot ride the generic Query pipeline because each
/// matches a child collection (licences, insurances) or a person inside the company.
/// </summary>
public class BackflowTesterSearchDto
{
    public string? BpatLicenseNumber { get; set; }

    public string? FireLicenseNumber { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    public string? UserEmail { get; set; }

    public string? ContactName { get; set; }

    public string? CellNumber { get; set; }

    /// <summary>
    /// Query-string form for the proxy hop. EnvirotraxApiClient drops blank values itself.
    /// </summary>
    public IDictionary<string, string> ToParameters()
    {
        return new Dictionary<string, string>
        {
            [nameof(BpatLicenseNumber)] = BpatLicenseNumber ?? string.Empty,
            [nameof(FireLicenseNumber)] = FireLicenseNumber ?? string.Empty,
            [nameof(InsurancePolicyNumber)] = InsurancePolicyNumber ?? string.Empty,
            [nameof(UserEmail)] = UserEmail ?? string.Empty,
            [nameof(ContactName)] = ContactName ?? string.Empty,
            [nameof(CellNumber)] = CellNumber ?? string.Empty
        };
    }
}
