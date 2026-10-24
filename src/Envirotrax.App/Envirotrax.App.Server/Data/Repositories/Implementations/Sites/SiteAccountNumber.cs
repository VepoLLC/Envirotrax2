using System.Text;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public static class SiteAccountNumber
{
    public static string CorrectWithHyphens(string accountNumber)
    {
        if (accountNumber.Contains('-'))
        {
            return accountNumber;
        }

        var corrected = new StringBuilder();

        foreach (var character in accountNumber)
        {
            if (!char.IsDigit(character))
            {
                continue;
            }

            corrected.Append(character);

            if (corrected.Length == 4 || corrected.Length == 9)
            {
                corrected.Append('-');
            }
        }

        var correctedAccountNumber = corrected.ToString().TrimEnd('-');

        if (correctedAccountNumber.Length == 0)
        {
            return accountNumber;
        }

        return correctedAccountNumber;
    }
}
