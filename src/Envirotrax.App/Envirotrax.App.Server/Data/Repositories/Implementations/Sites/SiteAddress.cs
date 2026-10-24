namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public static class SiteAddress
{
    private const string SuitePrefix = "ste";

    private static readonly string[] SuiteNumberPrefixes = ["#", "suite", "ste", "stuite", "bldg", "blg", "building", "apartment", "apt"];

    public static string Clean(string address)
    {
        return address.Replace(".", string.Empty).Replace(",", string.Empty).Trim();
    }

    public static string GetStreetNumber(string address)
    {
        var lastDigitIndex = -1;

        for (var index = 0; index < address.Length; index++)
        {
            var character = address[index];
            var isLastCharacter = index == address.Length - 1;

            if ((isLastCharacter || char.IsWhiteSpace(character)) && lastDigitIndex >= 0)
            {
                if (isLastCharacter)
                {
                    return address;
                }

                return address.Substring(0, lastDigitIndex + 1);
            }

            if (char.IsDigit(character))
            {
                lastDigitIndex = index;
            }
        }

        return string.Empty;
    }

    public static string GetStreetName(string address)
    {
        var streetNumber = GetStreetNumber(address);
        var streetName = address.Substring(streetNumber.Length).Trim();
        var suiteNumber = GetSuiteNumber(streetName);

        if (suiteNumber.Length == 0)
        {
            return streetName;
        }

        var streetNameLength = streetName.Length - suiteNumber.Length - 1;

        if (streetNameLength < 0)
        {
            return string.Empty;
        }

        return streetName.Substring(0, streetNameLength);
    }

    public static string GetSuiteNumber(string address)
    {
        var trimmedAddress = address.Trim();
        var lowerAddress = trimmedAddress.ToLowerInvariant();
        var suiteIndex = -1;

        for (var index = lowerAddress.Length - 1; index >= 0; index--)
        {
            var remainder = lowerAddress.Substring(index);

            foreach (var prefix in SuiteNumberPrefixes)
            {
                if (remainder.StartsWith(prefix, StringComparison.Ordinal) && IsSuitePrefixAt(lowerAddress, index, prefix))
                {
                    suiteIndex = index;
                }
            }
        }

        if (suiteIndex < 0)
        {
            return string.Empty;
        }

        return trimmedAddress.Substring(suiteIndex);
    }

    private static bool IsSuitePrefixAt(string lowerAddress, int index, string prefix)
    {
        if (prefix != SuitePrefix)
        {
            return true;
        }

        var nextIndex = index + SuitePrefix.Length;

        if (nextIndex >= lowerAddress.Length)
        {
            return false;
        }

        return !char.IsLetter(lowerAddress[nextIndex]);
    }
}
