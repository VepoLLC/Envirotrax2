
namespace Envirotrax.Website.Configuration;

public class LegacyUrlRedirectOptions
{
    public Dictionary<string, string> Overrides { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
