namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowComplianceHistoryColors
{
    private static readonly string[] Colors = ["20A845", "207CE5"];

    public static Dictionary<int, string> BuildYearColorMap(IEnumerable<int> years)
    {
        var orderedYears = years.Distinct().OrderBy(y => y).ToList();

        return orderedYears
            .Select((year, index) => (year, color: Colors[index % Colors.Length]))
            .ToDictionary(x => x.year, x => x.color);
    }
}
