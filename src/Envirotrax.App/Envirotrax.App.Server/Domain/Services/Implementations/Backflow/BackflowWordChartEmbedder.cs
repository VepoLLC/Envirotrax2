using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// Replaces a "{{placeholder}}" run in a MiniWord-produced document with a floating chart anchor sized
// to exactly fill its host table cell — shared by any Backflow Word builder whose template reserves a
// single (non-vertically-merged) cell for a chart, e.g. BackflowComplianceHistoryWordBuilder and
// BackflowNewRemovedWordBuilder. BackflowComplianceReportWordBuilder's doughnut cell is vertically
// merged across several rows and keeps its own more involved host-cell-sizing logic for that reason.
internal static class BackflowWordChartEmbedder
{
    private const long EmuPerTwip = 635L;

    public static void EmbedChart(MainDocumentPart mainPart, string placeholderText, string chartName, uint docPrId, C.ChartSpace chartSpace)
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
}
