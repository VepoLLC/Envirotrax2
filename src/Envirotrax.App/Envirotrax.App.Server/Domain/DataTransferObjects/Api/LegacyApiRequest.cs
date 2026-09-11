namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class LegacyApiRequest
{
    public string? UserId { get; set; }
    public string? ApiKey { get; set; }
    public string? RequestType { get; set; }
    public string? TableName { get; set; }
    public string? SelectFields { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderByDirection { get; set; }

    // Parsed so the values are visible to the endpoint, but neither is honoured yet: V1 deflates
    // the body for CompressResponse and renders JSON for FormatType=JSON. Both are tracked as
    // outstanding compatibility differences.
    public string? CompressResponse { get; set; }
    public string? FormatType { get; set; }

    /// <summary>
    /// The names V1 excludes when scanning the form for Where* criteria.
    /// </summary>
    private static readonly string[] ReservedNames =
    {
        "USERID", "APIKEY", "REQUESTTYPE", "TABLENAME", "SELECTFIELDS", "DATA",
        "__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION"
    };

    /// <summary>
    /// Every non-reserved, non-empty form parameter. V1 offers each of these to the criteria
    /// resolver and ignores the ones it does not recognise.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, string>> OtherParameters { get; private set; } = Array.Empty<KeyValuePair<string, string>>();

    /// <summary>
    /// Splits SelectFields the way V1 does: comma separated and trimmed, keeping the caller's order
    /// and any duplicate entries.
    /// </summary>
    public IReadOnlyList<string> ParseSelectFields()
    {
        if (string.IsNullOrEmpty(SelectFields))
        {
            return Array.Empty<string>();
        }

        return SelectFields
            .Split(',')
            .Select(field => field.Trim())
            .ToList();
    }

    /// <summary>
    /// Reads the fixed V1 parameters off the posted form. Values are kept as sent: V1 coerces them
    /// later, per parameter, and reproducing that coercion here would change which value wins.
    /// Lookups are case-insensitive because V1 reads from a NameValueCollection.
    /// </summary>
    public static LegacyApiRequest Parse(IFormCollection form)
    {
        var other = new List<KeyValuePair<string, string>>();

        foreach (var key in form.Keys)
        {
            var value = form[key].ToString();

            if (value == "" || ReservedNames.Contains(key.ToUpperInvariant()))
            {
                continue;
            }

            other.Add(new KeyValuePair<string, string>(key, value));
        }

        return new LegacyApiRequest
        {
            OtherParameters = other,
            UserId = Read(form, "UserID"),
            ApiKey = Read(form, "ApiKey"),
            RequestType = Read(form, "RequestType"),
            TableName = Read(form, "TableName"),
            SelectFields = Read(form, "SelectFields"),
            OrderBy = Read(form, "OrderBy"),
            OrderByDirection = Read(form, "OrderByDirection"),
            CompressResponse = Read(form, "CompressResponse"),
            FormatType = Read(form, "FormatType")
        };
    }

    /// <summary>
    /// Returns true when both credential fields were supplied, using V1's exact test
    /// (not null and not empty). Whitespace-only values count as supplied, so they fall through
    /// to authentication and produce 201 rather than 200 - matching V1.
    /// </summary>
    public bool HasCredentials()
    {
        return UserId != null
            && ApiKey != null
            && UserId != ""
            && ApiKey != "";
    }

    private static string? Read(IFormCollection form, string name)
    {
        foreach (var key in form.Keys)
        {
            if (string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
            {
                return form[key].ToString();
            }
        }

        return null;
    }
}
