using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using MiniSoftware;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowComplianceHistoryWordBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Word.Backflow.BackflowComplianceHistory.docx";
    private const int BarMaxBlocks = 20;

    // Charts read chronologically (oldest to newest) and cap at the most recent 48 months, matching
    // backflow-compliance-history-tab.component.ts's buildCharts; the table below still shows everything.
    private const int ChartMonthWindow = 48;

    public static byte[] Build(BackflowComplianceHistoryDto history)
    {
        var templateBytes = ReadTemplateBytes();

        // Most-recent-first, matching the on-screen table's reversedPoints.
        var points = history.Points.AsEnumerable().Reverse().ToList();
        var yearColors = BackflowComplianceHistoryColors.BuildYearColorMap(points.Select(p => p.Year));

        var value = new Dictionary<string, object>
        {
            ["Points"] = points
                .Select(point => new Dictionary<string, object>
                {
                    ["month"] = point.Label.Split(' ')[0],
                    ["year"] = point.Year,
                    ["total"] = point.Total,
                    ["compliant"] = point.Compliant,
                    ["percentage"] = FormatPercentage(point.Percentage),
                    ["bar"] = BuildBar(point.Percentage, yearColors[point.Year])
                })
                .ToList()
        };

        using var output = new MemoryStream();
        MiniWord.SaveAsByTemplate(output, templateBytes, value);

        AddCharts(output, history.Points);

        return output.ToArray();
    }

    private static string FormatPercentage(double percentage)
    {
        return percentage.ToString("0", CultureInfo.InvariantCulture);
    }

    // Word has no native data-bar equivalent to Excel's, so a solid block of colored characters
    // proportional to the percentage approximates the .reportbar gradient used on-screen and in PDF/Excel.
    private static MiniWordColorText BuildBar(double percentage, string hexColor)
    {
        var blockCount = (int)Math.Round(percentage / 100 * BarMaxBlocks);
        blockCount = Math.Clamp(blockCount, percentage > 0 ? 1 : 0, BarMaxBlocks);

        return new MiniWordColorText
        {
            Text = new string('█', blockCount),
            FontColor = hexColor,
            HighlightColor = "auto"
        };
    }

    private static void AddCharts(MemoryStream documentStream, IReadOnlyList<BackflowComplianceHistoryPointDto> points)
    {
        var chartPoints = points.Count > ChartMonthWindow
            ? points.Skip(points.Count - ChartMonthWindow).ToList()
            : points;

        documentStream.Position = 0;

        using (var doc = WordprocessingDocument.Open(documentStream, true))
        {
            var mainPart = doc.MainDocumentPart!;

            if (chartPoints.Count > 0)
            {
                var categories = chartPoints.Select(p => p.Label).ToList();

                BackflowWordChartEmbedder.EmbedChart(mainPart, "{{barChart}}", "Count Chart", docPrId: 1, BackflowComplianceHistoryChartHelper.BuildBarChartSpace(
                    categories,
                    chartPoints.Select(p => p.Total).ToList(),
                    chartPoints.Select(p => p.Compliant).ToList(),
                    chartPoints.Select(p => p.NonCompliant).ToList()));

                BackflowWordChartEmbedder.EmbedChart(mainPart, "{{lineChart}}", "Percent Chart", docPrId: 2, BackflowComplianceHistoryChartHelper.BuildLineChartSpace(
                    categories,
                    chartPoints.Select(p => p.Percentage).ToList()));
            }

            MiniWordDocumentFixup.FixColoredTextRunProperties(mainPart);
            mainPart.Document.Save();
        }

        documentStream.Position = 0;
    }

    private static byte[] ReadTemplateBytes()
    {
        using var templateStream = typeof(BackflowComplianceHistoryWordBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Word template embedded resource '{TemplateResourceName}' was not found.");

        using var buffer = new MemoryStream();
        templateStream.CopyTo(buffer);

        return buffer.ToArray();
    }
}
