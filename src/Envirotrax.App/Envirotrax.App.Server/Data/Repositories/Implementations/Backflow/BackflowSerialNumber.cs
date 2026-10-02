namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public static class BackflowSerialNumber
{
    public static bool IsValid(string serialNumber)
    {
        return serialNumber.Length >= 2 && serialNumber.Any(char.IsNumber);
    }

    public static string ToLetterWildcardPattern(string serialNumber)
    {
        return string.Concat(serialNumber.Select(character =>
            char.IsDigit(character) ? character.ToString()
            : char.IsLetter(character) ? "_"
            : "%"));
    }

    public static string ToBaseNumber(string serialNumber)
    {
        var digits = string.Concat(serialNumber.Where(char.IsDigit));
        var leadingZeroCount = digits.TakeWhile(digit => digit == '0').Count();

        return leadingZeroCount == digits.Length ? digits : digits[leadingZeroCount..];
    }

    public static bool IsBaseNumberMatch(string serialNumber, string candidateSerialNumber)
    {
        if (ToBaseNumber(serialNumber) != ToBaseNumber(candidateSerialNumber))
        {
            return false;
        }

        if (char.IsNumber(serialNumber[0]) || char.IsNumber(candidateSerialNumber[0]))
        {
            return true;
        }

        return char.IsLetter(candidateSerialNumber[0]) && serialNumber[0] == candidateSerialNumber[0];
    }
}
