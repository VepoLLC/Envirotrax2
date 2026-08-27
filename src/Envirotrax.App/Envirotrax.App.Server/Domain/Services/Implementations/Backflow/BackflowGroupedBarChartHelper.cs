using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// Builds a generic multi-series clustered column chart, shared by any Backflow report needing a
// grouped bar chart (compliance history's Total/Compliant/Non-Compliant, new/removed's
// Created/Removed, ...) — injected as native OOXML DrawingML directly into the Excel/Word package
// already produced, the same technique as BackflowComplianceChartHelper's doughnut.
internal static class BackflowGroupedBarChartHelper
{
    public static C.ChartSpace BuildChartSpace(IReadOnlyList<string> categories, IReadOnlyList<(string Name, string Color, IReadOnlyList<int> Values)> series)
    {
        const uint catAxisId = 111111111;
        const uint valAxisId = 222222222;

        var barChart = new C.BarChart(
            new C.BarDirection { Val = C.BarDirectionValues.Column },
            new C.BarGrouping { Val = C.BarGroupingValues.Clustered },
            new C.VaryColors { Val = false });

        for (var i = 0; i < series.Count; i++)
        {
            barChart.AppendChild(BuildSeries((uint)i, series[i].Name, series[i].Color, categories, series[i].Values));
        }

        barChart.AppendChild(new C.GapWidth { Val = 60 });
        barChart.AppendChild(new C.Overlap { Val = -10 });
        barChart.AppendChild(new C.AxisId { Val = catAxisId });
        barChart.AppendChild(new C.AxisId { Val = valAxisId });

        var plotArea = new C.PlotArea(
            new C.Layout(),
            barChart,
            BackflowChartAxisHelper.BuildCategoryAxis(catAxisId, valAxisId),
            BackflowChartAxisHelper.BuildValueAxis(valAxisId, catAxisId),
            new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));

        var chart = new C.Chart(
            new C.AutoTitleDeleted { Val = true },
            plotArea,
            new C.Legend(new C.LegendPosition { Val = C.LegendPositionValues.Top }, new C.Overlay { Val = false }),
            new C.PlotVisibleOnly { Val = true });

        return new C.ChartSpace(chart, new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));
    }

    private static C.BarChartSeries BuildSeries(uint index, string name, string color, IReadOnlyList<string> categories, IReadOnlyList<int> values)
    {
        return new C.BarChartSeries(
            new C.Index { Val = index },
            new C.Order { Val = index },
            new C.SeriesText(new C.NumericValue(name)),
            new C.ChartShapeProperties(new A.SolidFill(new A.RgbColorModelHex { Val = color })),
            new C.CategoryAxisData(BackflowChartAxisHelper.BuildStringLiteral(categories)),
            new C.Values(BackflowChartAxisHelper.BuildNumberLiteral(values.Select(v => (double)v).ToList())));
    }
}
