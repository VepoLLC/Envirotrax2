namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// The compliance history table colors each row by an alternating green/blue scheme per year (oldest
// year green, then blue, ...) so rows visually group by year — matching
// backflow-compliance-history-tab.component.ts's buildYearBarColors. Shared by the PDF/Excel/Word
// exports so the alternation logic isn't duplicated three times.
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
