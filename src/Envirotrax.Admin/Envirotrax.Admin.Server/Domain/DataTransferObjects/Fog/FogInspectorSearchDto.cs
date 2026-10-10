
namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// The person-level subset of the App's FogInspectorSearchDto that the admin window sends. Each one matches a
/// person inside the company rather than a column on the company row, so it cannot ride the generic Query pipeline.
/// </summary>
public class FogInspectorSearchDto
{
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
            [nameof(UserEmail)] = UserEmail ?? string.Empty,
            [nameof(ContactName)] = ContactName ?? string.Empty,
            [nameof(CellNumber)] = CellNumber ?? string.Empty
        };
    }
}
