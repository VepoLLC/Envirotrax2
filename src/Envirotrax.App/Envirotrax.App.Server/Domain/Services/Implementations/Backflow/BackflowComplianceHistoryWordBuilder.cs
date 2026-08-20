using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using MiniSoftware;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowComplianceHistoryWordBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Word.Backflow.BackflowComplianceHistory.docx";
    private const int BarMaxBlocks = 20;
    private const long EmuPerTwip = 635L;

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

    // MiniWord has no chart support, so — mirroring BackflowComplianceReportWordBuilder — the two
    // charts are injected as native OOXML DrawingML chart parts directly into the document MiniWord
    // already produced, replacing the literal "{{barChart}}"/"{{lineChart}}" placeholders the template
    // reserves for them (MiniWord only rewrites recognized {{tag}} placeholders, so these unrecognized
    // ones survive template processing untouched). Also applies MiniWordDocumentFixup here, same as
    // the other Word builders, since it must run after the substitutions above regardless of charts.
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

                EmbedChart(mainPart, "{{barChart}}", "Count Chart", docPrId: 1, BackflowComplianceHistoryChartHelper.BuildBarChartSpace(
                    categories,
                    chartPoints.Select(p => p.Total).ToList(),
                    chartPoints.Select(p => p.Compliant).ToList(),
                    chartPoints.Select(p => p.NonCompliant).ToList()));

                EmbedChart(mainPart, "{{lineChart}}", "Percent Chart", docPrId: 2, BackflowComplianceHistoryChartHelper.BuildLineChartSpace(
                    categories,
                    chartPoints.Select(p => p.Percentage).ToList()));
            }

            MiniWordDocumentFixup.FixColoredTextRunProperties(mainPart);
            mainPart.Document.Save();
        }

        documentStream.Position = 0;
    }

    // Replaces a placeholder run's text with a floating chart anchor sized to exactly fill its host
    // table cell — same technique as BackflowComplianceReportWordBuilder.AddComplianceChart.
    private static void EmbedChart(MainDocumentPart mainPart, string placeholderText, string chartName, uint docPrId, C.ChartSpace chartSpace)
    {
        var chartPart = mainPart.AddNewPart<ChartPart>();
        chartPart.ChartSpace = chartSpace;
        chartPart.ChartSpace.Save();

        var relationshipId = mainPart.GetIdOfPart(chartPart);

        var body = mainPart.Document.Body!;
        var placeholder = body.Descendants<Text>().First(t => t.Text == placeholderText);
        var run = (Run)placeholder.Parent!;

        var (widthEmu, heightEmu) = GetHostCellSizeEmu(placeholder);

        var anchor = new Wp.Anchor(
            new Wp.SimplePosition { X = 0, Y = 0 },
            new Wp.HorizontalPosition(new Wp.PositionOffset("0")) { RelativeFrom = Wp.HorizontalRelativePositionValues.Column },
            new Wp.VerticalPosition(new Wp.PositionOffset("0")) { RelativeFrom = Wp.VerticalRelativePositionValues.Paragraph },
            new Wp.Extent { Cx = widthEmu, Cy = heightEmu },
            new Wp.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
            new Wp.WrapNone(),
            new Wp.DocProperties { Id = docPrId, Name = chartName },
            new Wp.NonVisualGraphicFrameDrawingProperties(),
            new A.Graphic(new A.GraphicData(new C.ChartReference { Id = relationshipId })
            {
                Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart"
            }))
        {
            DistanceFromTop = 0,
            DistanceFromBottom = 0,
            DistanceFromLeft = 0,
            DistanceFromRight = 0,
            SimplePos = false,
            RelativeHeight = 1,
            BehindDoc = false,
            Locked = false,
            LayoutInCell = true,
            AllowOverlap = true
        };

        run.RemoveAllChildren<Text>();
        run.AppendChild(new Drawing(anchor));
    }

    // Sizes the chart to exactly match its host cell — the template reserves one table cell for each
    // chart, and however that cell gets resized by future template edits, the chart should always fill
    // it rather than a hardcoded EMU size going stale.
    private static (long WidthEmu, long HeightEmu) GetHostCellSizeEmu(Text placeholder)
    {
        var cell = placeholder.Ancestors<TableCell>().First();
        var row = cell.Ancestors<TableRow>().First();

        var widthTwips = cell.TableCellProperties?.TableCellWidth?.Width?.Value is string w && int.TryParse(w, out var parsedWidth)
            ? parsedWidth
            : 0;

        var heightTwips = row.TableRowProperties?.GetFirstChild<TableRowHeight>()?.Val?.Value is uint height ? (int)height : 0;

        return (widthTwips * EmuPerTwip, heightTwips * EmuPerTwip);
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
