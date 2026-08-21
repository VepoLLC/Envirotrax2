using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using MiniSoftware;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowComplianceReportWordBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Word.Backflow.BackflowComplianceReport.docx";
    private const string ChartPlaceholderText = "{{complianceChart}}";
    private const string BarPlaceholderText = "{{Requirements.bar}}";
    private const long EmuPerTwip = 635L;

    // Conservative estimate of a block-character glyph's width, used to size the bar so it always
    // fits on one line within its column — however wide the template's own "bar" column is — instead
    // of a fixed block count that only fit the column width the template happened to have at the time.
    private const int TwipsPerBlockEstimate = 140;
    private const double BarCellWidthSafetyFactor = 0.85;
    private const int FallbackBarMaxBlocks = 20;

    private const string CompliantColor = "20A845";

    public static byte[] Build(BackflowComplianceReportDto report, bool ignoreLast30Days)
    {
        var templateBytes = ReadTemplateBytes();
        var barMaxBlocks = GetBarMaxBlocks(templateBytes);

        var value = new Dictionary<string, object>
        {
            ["ignoreLast30Days"] = ignoreLast30Days ? "Yes" : "No",
            ["totalActive"] = report.TotalActive,
            ["compliantText"] = $"{report.Compliant} ({FormatPercentage(report.CompliantPercentage)}%)",
            ["nonCompliantText"] = $"{report.NonCompliant} ({FormatPercentage(report.NonCompliantPercentage)}%)",
            ["Requirements"] = report.Requirements
                .Select(requirement => new Dictionary<string, object>
                {
                    ["propertyType"] = requirement.PropertyType,
                    ["assemblyType"] = requirement.AssemblyType,
                    ["hazardType"] = requirement.HazardType,
                    ["ossf"] = requirement.HasSiteOssf ? "1" : "0",
                    ["auxWater"] = requirement.AuxWaterSupply ? "1" : "0",
                    ["renewalYears"] = requirement.RenewalYears,
                    ["active"] = requirement.Active,
                    ["compliant"] = requirement.Compliant,
                    ["percentage"] = FormatPercentage(requirement.Percentage),
                    ["bar"] = BuildBar(requirement.Percentage, barMaxBlocks)
                })
                .ToList()
        };

        using var output = new MemoryStream();
        MiniWord.SaveAsByTemplate(output, templateBytes, value);

        AddComplianceChart(output, report.Compliant, report.NonCompliant);

        return output.ToArray();
    }

    private static string FormatPercentage(double percentage)
    {
        return percentage.ToString("0", CultureInfo.InvariantCulture);
    }

    // Word has no native data-bar equivalent to Excel's, so a solid block of colored characters
    // proportional to the percentage approximates the .reportbar gradient used on-screen, in the
    // PDF, and in Excel — used for the per-requirement rows (the overall split gets a real chart below).
    private static MiniWordColorText BuildBar(double percentage, int maxBlocks)
    {
        var blockCount = (int)Math.Round(percentage / 100 * maxBlocks);
        blockCount = Math.Clamp(blockCount, percentage > 0 ? 1 : 0, maxBlocks);

        return new MiniWordColorText
        {
            Text = new string('█', blockCount),
            FontColor = CompliantColor,
            HighlightColor = "auto"
        };
    }

    // Measures the template's own "bar" column so the block-character bar always fits on one line
    // within it — a fixed block count only stays correct until someone resizes that column again.
    private static int GetBarMaxBlocks(byte[] templateBytes)
    {
        using var stream = new MemoryStream(templateBytes);
        using var doc = WordprocessingDocument.Open(stream, false);
        var body = doc.MainDocumentPart!.Document.Body!;

        var placeholder = body.Descendants<Text>().FirstOrDefault(t => t.Text == BarPlaceholderText);
        var widthTwips = placeholder?.Ancestors<TableCell>().FirstOrDefault()?.TableCellProperties?.TableCellWidth?.Width?.Value is string w
            && int.TryParse(w, out var parsed)
            ? parsed
            : 0;

        if (widthTwips <= 0)
        {
            return FallbackBarMaxBlocks;
        }

        return Math.Max(1, (int)(widthTwips * BarCellWidthSafetyFactor / TwipsPerBlockEstimate));
    }

    // MiniWord has no chart support either, so — mirroring the Excel builder — the doughnut chart is
    // injected as a native OOXML DrawingML chart part directly into the document MiniWord already
    // produced. It replaces the literal "{{complianceChart}}" placeholder the template reserves for it
    // (MiniWord only rewrites recognized {{tag}} placeholders, so an unrecognized one like this survives
    // template processing untouched). The chart is anchored as a FLOATING drawing (not inline) so it
    // doesn't force its narrow host cell to grow to fit its width — see BackflowComplianceReport.docx.
    private static void AddComplianceChart(MemoryStream documentStream, int compliant, int nonCompliant)
    {
        documentStream.Position = 0;

        using var doc = WordprocessingDocument.Open(documentStream, true);
        var mainPart = doc.MainDocumentPart!;

        var chartPart = mainPart.AddNewPart<ChartPart>();
        chartPart.ChartSpace = BackflowComplianceChartHelper.BuildChartSpace(compliant, nonCompliant, includeLegend: false);
        chartPart.ChartSpace.Save();

        var relationshipId = mainPart.GetIdOfPart(chartPart);

        var body = mainPart.Document.Body!;
        var placeholder = body.Descendants<Text>().First(t => t.Text == ChartPlaceholderText);
        var run = (Run)placeholder.Parent!;

        var (widthEmu, heightEmu) = GetHostCellSizeEmu(placeholder);

        var anchor = new Wp.Anchor(
            new Wp.SimplePosition { X = 0, Y = 0 },
            new Wp.HorizontalPosition(new Wp.PositionOffset("0")) { RelativeFrom = Wp.HorizontalRelativePositionValues.Column },
            new Wp.VerticalPosition(new Wp.PositionOffset("0")) { RelativeFrom = Wp.VerticalRelativePositionValues.Paragraph },
            new Wp.Extent { Cx = widthEmu, Cy = heightEmu },
            new Wp.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
            new Wp.WrapNone(),
            new Wp.DocProperties { Id = 1, Name = "Compliance Chart" },
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

        MiniWordDocumentFixup.FixColoredTextRunProperties(mainPart);

        mainPart.Document.Save();
        documentStream.Position = 0;
    }

    // Sizes the chart to exactly match its host cell — the template reserves one (possibly vertically
    // merged) table cell for the chart, and however that cell gets resized by future template edits,
    // the chart should always fill it rather than a hardcoded EMU size going stale.
    private static (long WidthEmu, long HeightEmu) GetHostCellSizeEmu(Text placeholder)
    {
        var cell = placeholder.Ancestors<TableCell>().First();
        var row = cell.Ancestors<TableRow>().First();
        var table = row.Ancestors<Table>().First();

        var widthTwips = cell.TableCellProperties?.TableCellWidth?.Width?.Value is string w && int.TryParse(w, out var parsedWidth)
            ? parsedWidth
            : 0;

        var cellIndex = row.Elements<TableCell>().ToList().IndexOf(cell);
        var rows = table.Elements<TableRow>().ToList();
        var rowIndex = rows.IndexOf(row);

        var heightTwips = GetRowHeightTwips(row);

        for (var i = rowIndex + 1; i < rows.Count; i++)
        {
            var candidateCell = rows[i].Elements<TableCell>().ElementAtOrDefault(cellIndex);
            var isContinuation = candidateCell?.TableCellProperties?.VerticalMerge != null
                && candidateCell.TableCellProperties!.VerticalMerge!.Val?.Value != MergedCellValues.Restart;

            if (!isContinuation)
            {
                break;
            }

            heightTwips += GetRowHeightTwips(rows[i]);
        }

        return (widthTwips * EmuPerTwip, heightTwips * EmuPerTwip);
    }

    private static int GetRowHeightTwips(TableRow row)
    {
        return row.TableRowProperties?.GetFirstChild<TableRowHeight>()?.Val?.Value is uint height ? (int)height : 0;
    }

    private static byte[] ReadTemplateBytes()
    {
        using var templateStream = typeof(BackflowComplianceReportWordBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Word template embedded resource '{TemplateResourceName}' was not found.");

        using var buffer = new MemoryStream();
        templateStream.CopyTo(buffer);

        return buffer.ToArray();
    }
}
