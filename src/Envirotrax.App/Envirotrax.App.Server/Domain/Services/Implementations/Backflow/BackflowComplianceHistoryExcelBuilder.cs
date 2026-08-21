using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Ss = DocumentFormat.OpenXml.Spreadsheet;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowComplianceHistoryExcelBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Excel.Backflow.BackflowComplianceHistory.xlsx";
    private const int TemplateRow = 29;

    // Charts read chronologically (oldest to newest) and cap at the most recent 48 months, matching
    // backflow-compliance-history-tab.component.ts's buildCharts; the table below still shows everything.
    private const int ChartMonthWindow = 48;

    public static byte[] Build(BackflowComplianceHistoryDto history)
    {
        using var templateStream = typeof(BackflowComplianceHistoryExcelBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Excel template embedded resource '{TemplateResourceName}' was not found.");

        using var workbook = new XLWorkbook(templateStream);
        var ws = workbook.Worksheet("Report");

        // Most-recent-first, matching the on-screen table's reversedPoints.
        var points = history.Points.AsEnumerable().Reverse().ToList();

        if (points.Count == 0)
        {
            ws.Row(TemplateRow).Delete();
        }
        else
        {
            WritePoints(ws, points);
        }

        using var output = new MemoryStream();
        workbook.SaveAs(output);

        AddCharts(output, "Report", history.Points);

        return output.ToArray();
    }

    private static void WritePoints(IXLWorksheet ws, IReadOnlyList<BackflowComplianceHistoryPointDto> points)
    {
        CloneRowsBelow(ws, TemplateRow, points.Count - 1);

        for (var i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var row = TemplateRow + i;

            ws.Cell(row, 1).Value = point.Label.Split(' ')[0];
            ws.Cell(row, 2).Value = point.Year;

            ws.Cell(row, 3).Value = point.Percentage;
            ws.Cell(row, 3).Style.NumberFormat.Format = ";;;";

            ws.Cell(row, 4).Value = point.Total;
            ws.Cell(row, 5).Value = point.Compliant;

            ws.Cell(row, 6).Value = point.Percentage;
            ws.Cell(row, 6).Style.NumberFormat.Format = "0\"%\"";
        }

        var yearColors = BackflowComplianceHistoryColors.BuildYearColorMap(points.Select(p => p.Year));

        // Data bars apply to one contiguous range at a time, so each same-year run of rows gets its
        // own conditional format call, alternating colors the same way the rows themselves do.
        var runStart = 0;
        for (var i = 1; i <= points.Count; i++)
        {
            var runEnded = i == points.Count || points[i].Year != points[runStart].Year;

            if (!runEnded)
            {
                continue;
            }

            var color = yearColors[points[runStart].Year];
            var firstRow = TemplateRow + runStart;
            var lastRow = TemplateRow + i - 1;

            ws.Range(firstRow, 3, lastRow, 3).AddConditionalFormat().DataBar(XLColor.FromHtml($"#{color}"));

            runStart = i;
        }
    }

    // Inserts `count` copies of `templateRow` immediately below it, each carrying the template row's style.
    // A count <= 0 (a single-item section) leaves the template row as-is.
    private static void CloneRowsBelow(IXLWorksheet ws, int templateRow, int count)
    {
        if (count <= 0)
        {
            return;
        }

        var source = ws.Row(templateRow);
        var inserted = source.InsertRowsBelow(count);

        foreach (var row in inserted)
        {
            source.CopyTo(row);
        }
    }

    // ClosedXML (0.105.0) doesn't expose a public API for creating charts, so the two charts are
    // injected as native OOXML DrawingML chart parts directly into the workbook ClosedXML already
    // produced — same technique as BackflowComplianceReportExcelBuilder's doughnut. They land in the
    // blank rows 2-27 the template reserves for them (row 2/15 are the section header labels).
    private static void AddCharts(MemoryStream workbookStream, string sheetName, IReadOnlyList<BackflowComplianceHistoryPointDto> points)
    {
        var chartPoints = points.Count > ChartMonthWindow
            ? points.Skip(points.Count - ChartMonthWindow).ToList()
            : points;

        if (chartPoints.Count == 0)
        {
            return;
        }

        var categories = chartPoints.Select(p => p.Label).ToList();

        workbookStream.Position = 0;

        using var doc = SpreadsheetDocument.Open(workbookStream, true);
        var workbookPart = doc.WorkbookPart!;
        var sheet = workbookPart.Workbook.Descendants<Ss.Sheet>().First(s => s.Name == sheetName);
        var sheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);

        var drawingsPart = sheetPart.AddNewPart<DrawingsPart>();

        var barChartPart = drawingsPart.AddNewPart<ChartPart>();
        barChartPart.ChartSpace = BackflowComplianceHistoryChartHelper.BuildBarChartSpace(
            categories,
            chartPoints.Select(p => p.Total).ToList(),
            chartPoints.Select(p => p.Compliant).ToList(),
            chartPoints.Select(p => p.NonCompliant).ToList());
        barChartPart.ChartSpace.Save();

        var lineChartPart = drawingsPart.AddNewPart<ChartPart>();
        lineChartPart.ChartSpace = BackflowComplianceHistoryChartHelper.BuildLineChartSpace(
            categories,
            chartPoints.Select(p => p.Percentage).ToList());
        lineChartPart.ChartSpace.Save();

        drawingsPart.WorksheetDrawing = BuildWorksheetDrawing(
            drawingsPart.GetIdOfPart(barChartPart),
            drawingsPart.GetIdOfPart(lineChartPart));
        drawingsPart.WorksheetDrawing.Save();

        var worksheet = sheetPart.Worksheet;
        var drawingElement = new Ss.Drawing { Id = sheetPart.GetIdOfPart(drawingsPart) };

        // CT_Worksheet has a strict child sequence — <drawing> must come before <tableParts>/<extLst>
        // if present, so insert relative to those rather than blindly appending.
        var insertBeforeElement = worksheet.Elements<Ss.TableParts>().FirstOrDefault()
            ?? (DocumentFormat.OpenXml.OpenXmlElement?)worksheet.Elements<Ss.WorksheetExtensionList>().FirstOrDefault();

        if (insertBeforeElement != null)
        {
            worksheet.InsertBefore(drawingElement, insertBeforeElement);
        }
        else
        {
            worksheet.Append(drawingElement);
        }

        worksheet.Save();
        workbookStream.Position = 0;
    }

    private static Xdr.WorksheetDrawing BuildWorksheetDrawing(string barChartRelationshipId, string lineChartRelationshipId)
    {
        var barAnchor = BuildChartAnchor(barChartRelationshipId, "Count Chart", fromRow: 2, toRow: 14);
        var lineAnchor = BuildChartAnchor(lineChartRelationshipId, "Percent Chart", fromRow: 15, toRow: 27);

        return new Xdr.WorksheetDrawing(barAnchor, lineAnchor);
    }

    // Anchored across the full column width (A-F) within the row range reserved for it.
    private static Xdr.TwoCellAnchor BuildChartAnchor(string chartRelationshipId, string name, int fromRow, int toRow)
    {
        var fromMarker = new Xdr.FromMarker(
            new Xdr.ColumnId("0"), new Xdr.ColumnOffset("0"),
            new Xdr.RowId(fromRow.ToString()), new Xdr.RowOffset("0"));

        var toMarker = new Xdr.ToMarker(
            new Xdr.ColumnId("6"), new Xdr.ColumnOffset("0"),
            new Xdr.RowId(toRow.ToString()), new Xdr.RowOffset("0"));

        var graphicFrame = new Xdr.GraphicFrame(
            new Xdr.NonVisualGraphicFrameProperties(
                new Xdr.NonVisualDrawingProperties { Id = (uint)(fromRow + 1), Name = name },
                new Xdr.NonVisualGraphicFrameDrawingProperties()),
            new Xdr.Transform(new A.Offset { X = 0, Y = 0 }, new A.Extents { Cx = 0, Cy = 0 }),
            new A.Graphic(new A.GraphicData(
                new C.ChartReference { Id = chartRelationshipId })
            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" }));

        return new Xdr.TwoCellAnchor(fromMarker, toMarker, graphicFrame, new Xdr.ClientData());
    }
}
