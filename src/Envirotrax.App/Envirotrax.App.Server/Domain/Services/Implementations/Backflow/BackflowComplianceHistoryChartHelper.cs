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
        const uint catAxisId = 111111111;
        const uint valAxisId = 222222222;

        var barChart = new C.BarChart(
            new C.BarDirection { Val = C.BarDirectionValues.Column },
            new C.BarGrouping { Val = C.BarGroupingValues.Clustered },
            new C.VaryColors { Val = false },
            BuildBarSeries(0, "Total", TotalColor, categories, total),
            BuildBarSeries(1, "Compliant", CompliantColor, categories, compliant),
            BuildBarSeries(2, "Non-Compliant", NonCompliantColor, categories, nonCompliant),
            new C.GapWidth { Val = 60 },
            new C.Overlap { Val = -10 },
            new C.AxisId { Val = catAxisId },
            new C.AxisId { Val = valAxisId });

        var plotArea = new C.PlotArea(
            new C.Layout(),
            barChart,
            BuildCategoryAxis(catAxisId, valAxisId),
            BuildValueAxis(valAxisId, catAxisId),
            new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));

        var chart = new C.Chart(
            new C.AutoTitleDeleted { Val = true },
            plotArea,
            new C.Legend(new C.LegendPosition { Val = C.LegendPositionValues.Top }, new C.Overlay { Val = false }),
            new C.PlotVisibleOnly { Val = true });

        return new C.ChartSpace(chart, new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));
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
            BuildCategoryAxis(catAxisId, valAxisId),
            BuildValueAxis(valAxisId, catAxisId, min: 0, max: 100),
            new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));

        var chart = new C.Chart(
            new C.AutoTitleDeleted { Val = true },
            plotArea,
            new C.PlotVisibleOnly { Val = true });

        return new C.ChartSpace(chart, new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));
    }

    private static C.BarChartSeries BuildBarSeries(uint index, string name, string color, IReadOnlyList<string> categories, IReadOnlyList<int> values)
    {
        return new C.BarChartSeries(
            new C.Index { Val = index },
            new C.Order { Val = index },
            new C.SeriesText(new C.NumericValue(name)),
            new C.ChartShapeProperties(new A.SolidFill(new A.RgbColorModelHex { Val = color })),
            new C.CategoryAxisData(BuildStringLiteral(categories)),
            new C.Values(BuildNumberLiteral(values.Select(v => (double)v).ToList())));
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
            new C.CategoryAxisData(BuildStringLiteral(categories)),
            new C.Values(BuildNumberLiteral(values)));
    }

    private static C.CategoryAxis BuildCategoryAxis(uint axisId, uint crossAxisId)
    {
        return new C.CategoryAxis(
            new C.AxisId { Val = axisId },
            new C.Scaling(new C.Orientation { Val = C.OrientationValues.MinMax }),
            new C.Delete { Val = false },
            new C.AxisPosition { Val = C.AxisPositionValues.Bottom },
            new C.TickLabelPosition { Val = C.TickLabelPositionValues.Low },
            new C.CrossingAxis { Val = crossAxisId });
    }

    private static C.ValueAxis BuildValueAxis(uint axisId, uint crossAxisId, double? min = null, double? max = null)
    {
        var scaling = new C.Scaling(new C.Orientation { Val = C.OrientationValues.MinMax });

        if (max.HasValue)
        {
            scaling.MaxAxisValue = new C.MaxAxisValue { Val = max.Value };
        }

        if (min.HasValue)
        {
            scaling.MinAxisValue = new C.MinAxisValue { Val = min.Value };
        }

        return new C.ValueAxis(
            new C.AxisId { Val = axisId },
            scaling,
            new C.Delete { Val = false },
            new C.AxisPosition { Val = C.AxisPositionValues.Left },
            new C.MajorGridlines(),
            new C.CrossingAxis { Val = crossAxisId });
    }

    private static C.StringLiteral BuildStringLiteral(IReadOnlyList<string> values)
    {
        var literal = new C.StringLiteral(new C.PointCount { Val = (uint)values.Count });

        for (var i = 0; i < values.Count; i++)
        {
            literal.Append(new C.StringPoint { Index = (uint)i, NumericValue = new C.NumericValue(values[i]) });
        }

        return literal;
    }

    private static C.NumberLiteral BuildNumberLiteral(IReadOnlyList<double> values)
    {
        var literal = new C.NumberLiteral(new C.FormatCode("General"), new C.PointCount { Val = (uint)values.Count });

        for (var i = 0; i < values.Count; i++)
        {
            literal.Append(new C.NumericPoint { Index = (uint)i, NumericValue = new C.NumericValue(values[i].ToString(System.Globalization.CultureInfo.InvariantCulture)) });
        }

        return literal;
    }
}
