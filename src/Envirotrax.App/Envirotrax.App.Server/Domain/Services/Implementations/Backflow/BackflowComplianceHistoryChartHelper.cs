using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// Builds the two DrawingML charts shared by the Excel and Word compliance history exports (a grouped
// column chart of Total/Compliant/Non-Compliant per month, and a line chart of % compliant per month) —
// matching backflow-compliance-history-tab.component.ts's countChartData/percentChartData. Neither
// ClosedXML nor MiniWord can create charts through their own APIs, so both builders inject these
// directly into the OOXML package they've already produced, the same way
// BackflowComplianceChartHelper does for the current-compliance doughnut.
internal static class BackflowComplianceHistoryChartHelper
{
    private const string TotalColor = "50A0FF";
    private const string CompliantColor = "3EC46E";
    private const string NonCompliantColor = "FF5A5A";
    private const string PercentLineColor = "2BBD4F";

    public static C.ChartSpace BuildBarChartSpace(IReadOnlyList<string> categories, IReadOnlyList<int> total, IReadOnlyList<int> compliant, IReadOnlyList<int> nonCompliant)
    {
        return BackflowGroupedBarChartHelper.BuildChartSpace(categories,
        [
            ("Total", TotalColor, total),
            ("Compliant", CompliantColor, compliant),
            ("Non-Compliant", NonCompliantColor, nonCompliant)
        ]);
    }

    public static C.ChartSpace BuildLineChartSpace(IReadOnlyList<string> categories, IReadOnlyList<double> percentages)
    {
        const uint catAxisId = 333333333;
        const uint valAxisId = 444444444;

        var lineChart = new C.LineChart(
            new C.Grouping { Val = C.GroupingValues.Standard },
            new C.VaryColors { Val = false },
            BuildLineSeries(0, "Percent Compliant", categories, percentages),
            new C.ShowMarker { Val = true },
            new C.AxisId { Val = catAxisId },
            new C.AxisId { Val = valAxisId });

        var plotArea = new C.PlotArea(
            new C.Layout(),
            lineChart,
            BackflowChartAxisHelper.BuildCategoryAxis(catAxisId, valAxisId),
            BackflowChartAxisHelper.BuildValueAxis(valAxisId, catAxisId, min: 0, max: 100),
            new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));

        var chart = new C.Chart(
            new C.AutoTitleDeleted { Val = true },
            plotArea,
            new C.PlotVisibleOnly { Val = true });

        return new C.ChartSpace(chart, new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));
    }

    private static C.LineChartSeries BuildLineSeries(uint index, string name, IReadOnlyList<string> categories, IReadOnlyList<double> values)
    {
        return new C.LineChartSeries(
            new C.Index { Val = index },
            new C.Order { Val = index },
            new C.SeriesText(new C.NumericValue(name)),
            new C.ChartShapeProperties(new A.Outline(new A.SolidFill(new A.RgbColorModelHex { Val = PercentLineColor })) { Width = 28575 }),
            new C.Marker(
                new C.Symbol { Val = C.MarkerStyleValues.Circle },
                new C.ChartShapeProperties(new A.SolidFill(new A.RgbColorModelHex { Val = PercentLineColor }))),
            new C.CategoryAxisData(BackflowChartAxisHelper.BuildStringLiteral(categories)),
            new C.Values(BackflowChartAxisHelper.BuildNumberLiteral(values)));
    }
}
