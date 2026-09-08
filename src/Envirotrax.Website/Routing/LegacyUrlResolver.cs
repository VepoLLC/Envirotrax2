
namespace Envirotrax.Website.Routing;

public static class LegacyUrlResolver
{
    private const string LegacyExtension = ".aspx";

    public static bool IsLegacyRequest(string path)
    {
        return path.EndsWith(LegacyExtension, StringComparison.OrdinalIgnoreCase);
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
