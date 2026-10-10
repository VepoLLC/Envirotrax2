
namespace Envirotrax.Website.Routing;

public static class LegacyUrlResolver
{
    private const string LegacyExtension = ".aspx";
    private const string PublicSearchLegacyPath = "/public_search";
    private const string PublicSearchLegacyIndexPath = "/public_search/index.aspx";
    private const string PublicSearchAppPath = "/public-search";

    // Printed on renewal letters and sent in renewal emails, so these keep working after V1 is retired.
    private const string RenewalOptInLegacyPath = "/renewal_opt_in.aspx";
    private const string RenewalEmailVerifyLegacyPath = "/renewal_email_verify.aspx";
    private const string RenewalUnsubscribeLegacyPath = "/renewal_unsubscribe.aspx";
    private const string RenewalOptInAppPath = "/renewal-opt-in";

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

    /// <summary>
    /// Builds the app URL for a legacy renewal opt-in page, or returns null when the path is not one.
    /// The opt-in page takes the site id in its path; the email links keep their query string as is.
    /// </summary>
    public static string? BuildRenewalOptInUrl(string appUrl, string normalizedPath, string? siteId, string queryString)
    {
        var url = appUrl.TrimEnd('/') + RenewalOptInAppPath;

        if (normalizedPath == RenewalOptInLegacyPath && !string.IsNullOrWhiteSpace(siteId))
        {
            return $"{url}/{Uri.EscapeDataString(siteId)}";
        }

        if (normalizedPath == RenewalEmailVerifyLegacyPath)
        {
            return $"{url}/verify-email{queryString}";
        }

        if (normalizedPath == RenewalUnsubscribeLegacyPath)
        {
            return $"{url}/unsubscribe{queryString}";
        }

        return null;
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
