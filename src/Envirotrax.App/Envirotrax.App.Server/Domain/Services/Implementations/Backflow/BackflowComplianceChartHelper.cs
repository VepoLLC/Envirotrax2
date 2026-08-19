using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// Builds the DrawingML chart shared by the Excel and Word compliance report exports. Neither
// ClosedXML nor MiniWord can create charts through their own APIs, so both builders inject this
// directly into the OOXML package they've already produced (see AddComplianceChart in
// BackflowComplianceReportExcelBuilder and BackflowComplianceReportWordBuilder).
internal static class BackflowComplianceChartHelper
{
    private const string CompliantColor = "20A845";
    private const string NonCompliantColor = "DC3545";

    // includeLegend: Excel draws its own colored-swatch legend alongside the chart (matching the
    // on-screen/PDF legend), so its chart omits the native one; Word has no such custom legend, so
    // its chart keeps the native one to still identify which color is which.
    public static C.ChartSpace BuildChartSpace(int compliant, int nonCompliant, bool includeLegend)
    {
        var categories = new C.StringLiteral(
            new C.PointCount { Val = 2 },
            new C.StringPoint { Index = 0, NumericValue = new C.NumericValue("Compliant") },
            new C.StringPoint { Index = 1, NumericValue = new C.NumericValue("Non-Compliant") });

        var values = new C.NumberLiteral(
            new C.FormatCode("General"),
            new C.PointCount { Val = 2 },
            new C.NumericPoint { Index = 0, NumericValue = new C.NumericValue(compliant.ToString()) },
            new C.NumericPoint { Index = 1, NumericValue = new C.NumericValue(nonCompliant.ToString()) });

        var series = new C.PieChartSeries(
            new C.Index { Val = 0 },
            new C.Order { Val = 0 },
            new C.DataPoint(
                new C.Index { Val = 0 },
                new C.ChartShapeProperties(new A.SolidFill(new A.RgbColorModelHex { Val = CompliantColor }))),
            new C.DataPoint(
                new C.Index { Val = 1 },
                new C.ChartShapeProperties(new A.SolidFill(new A.RgbColorModelHex { Val = NonCompliantColor }))),
            new C.DataLabels(
                new C.ShowLegendKey { Val = false },
                new C.ShowValue { Val = false },
                new C.ShowCategoryName { Val = false },
                new C.ShowSeriesName { Val = false },
                new C.ShowPercent { Val = true },
                new C.ShowBubbleSize { Val = false }),
            new C.CategoryAxisData(categories),
            new C.Values(values));

        var doughnutChart = new C.DoughnutChart(
            new C.VaryColors { Val = true },
            series,
            new C.FirstSliceAngle { Val = 0 },
            new C.HoleSize { Val = 60 });

        // No fill/no line on both the plot area and the chart area — otherwise Excel/Word draw their
        // default boxed border around the chart, making it look like a closed rectangle.
        var plotArea = new C.PlotArea(
            new C.Layout(),
            doughnutChart,
            new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));

        var chart = new C.Chart(new C.AutoTitleDeleted { Val = true }, plotArea);

        if (includeLegend)
        {
            chart.Append(new C.Legend(
                new C.LegendPosition { Val = C.LegendPositionValues.Right },
                new C.Overlay { Val = false }));
        }

        chart.Append(new C.PlotVisibleOnly { Val = true });

        var chartSpace = new C.ChartSpace(
            chart,
            new C.ShapeProperties(new A.NoFill(), new A.Outline(new A.NoFill())));

        return chartSpace;
    }
}
