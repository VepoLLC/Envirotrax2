using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public static class SiteInformationComparer
{
    public static bool HasTextChanged(List<(string? EnteredValue, string? SiteValue)> fields)
    {
        foreach (var field in fields)
        {
            var enteredValue = NormalizeText(field.EnteredValue);
            var siteValue = NormalizeText(field.SiteValue);

            if (!string.Equals(enteredValue, siteValue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static int? GetStateId(ReferencedStateDto? state)
    {
        if (state == null)
        {
            return null;
        }

        return state.Id;
    }

    private static string NormalizeText(string? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        return value.Trim();
    }
}
