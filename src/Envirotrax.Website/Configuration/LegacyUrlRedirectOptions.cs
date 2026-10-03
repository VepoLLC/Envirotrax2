
namespace Envirotrax.Website.Configuration;

public class LegacyUrlRedirectOptions
{
    public Dictionary<string, string> Overrides { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? SubdomainBaseDomain { get; set; }

    public List<string> ReservedSubdomains { get; set; } = [];
}
