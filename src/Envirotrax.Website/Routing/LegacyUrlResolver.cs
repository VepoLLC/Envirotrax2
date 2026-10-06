
namespace Envirotrax.Website.Routing;

public static class LegacyUrlResolver
{
    private const string LegacyExtension = ".aspx";
    private const string PublicSearchLegacyPath = "/public_search";
    private const string PublicSearchLegacyIndexPath = "/public_search/index.aspx";
    private const string PublicSearchAppPath = "/public-search";

    public static bool IsLegacyRequest(string path)
    {
        return path.EndsWith(LegacyExtension, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPublicSearchRequest(string normalizedPath)
    {
        return normalizedPath == PublicSearchLegacyPath || normalizedPath == PublicSearchLegacyIndexPath;
    }

    public static string? ExtractSubdomain(string host, string? baseDomain, IEnumerable<string> reservedSubdomains)
    {
        if (string.IsNullOrWhiteSpace(baseDomain))
        {
            return null;
        }

        var suffix = "." + baseDomain.Trim().TrimStart('.');

        if (!host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var subdomain = host[..^suffix.Length].ToLowerInvariant();

        if (subdomain.Length == 0
            || subdomain.Contains('.')
            || reservedSubdomains.Contains(subdomain, StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        return subdomain;
    }

    public static string BuildPublicSearchUrl(string appUrl, string? subdomain)
    {
        var url = appUrl.TrimEnd('/') + PublicSearchAppPath;

        return subdomain == null
            ? url
            : $"{url}?domain={Uri.EscapeDataString(subdomain)}";
    }

    public static string Normalize(string path)
    {
        var normalized = string.IsNullOrEmpty(path) ? "/" : path.ToLowerInvariant();

        if (!normalized.StartsWith('/'))
        {
            normalized = "/" + normalized;
        }

        if (normalized.Length > 1)
        {
            normalized = normalized.TrimEnd('/');
        }

        return normalized;
    }

    public static string ResolveCandidateTarget(string normalizedRequestPath, IReadOnlyDictionary<string, string> overrides)
    {
        if (overrides.TryGetValue(normalizedRequestPath, out var overrideTarget))
        {
            return Normalize(overrideTarget);
        }

        var withoutExtension = normalizedRequestPath[..^LegacyExtension.Length];
        var hyphenated = withoutExtension.Replace('_', '-');

        return Normalize(hyphenated);
    }
}
