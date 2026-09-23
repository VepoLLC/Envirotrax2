using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Ss = DocumentFormat.OpenXml.Spreadsheet;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowNewRemovedExcelBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Excel.Backflow.BackflowNewRemoved.xlsx";
    private const string CreatedColor = "F0922B";
    private const string RemovedColor = "5AC8E8";

    public static byte[] Build(BackflowNewRemovedReportDto report)
    {
        using var templateStream = typeof(BackflowNewRemovedExcelBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Excel template embedded resource '{TemplateResourceName}' was not found.");

        using var workbook = new XLWorkbook(templateStream);

        using var output = new MemoryStream();
        workbook.SaveAs(output);

        AddChart(output, "Report", report.Points);

        return output.ToArray();
    }

    // ClosedXML (0.105.0) doesn't expose a public API for creating charts, so the grouped bar chart is
    // injected as a native OOXML DrawingML chart part directly into the workbook ClosedXML already
    // produced. It lands in the blank rows 3-14 reserved for it in the template, below the row-2
    // section header.
    private static void AddChart(MemoryStream workbookStream, string sheetName, IReadOnlyList<BackflowNewRemovedPointDto> points)
    {
        if (points.Count == 0)
        {
            return;
        }

        var categories = points.Select(p => p.Label).ToList();

        workbookStream.Position = 0;

        using var doc = SpreadsheetDocument.Open(workbookStream, true);
        var workbookPart = doc.WorkbookPart!;
        var sheet = workbookPart.Workbook.Descendants<Ss.Sheet>().First(s => s.Name == sheetName);
        var sheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);

        var drawingsPart = sheetPart.AddNewPart<DrawingsPart>();
        var chartPart = drawingsPart.AddNewPart<ChartPart>();

        chartPart.ChartSpace = BackflowGroupedBarChartHelper.BuildChartSpace(categories,
        [
            ("Assemblies Created", CreatedColor, points.Select(p => p.Created).ToList()),
            ("Assemblies Removed", RemovedColor, points.Select(p => p.Removed).ToList())
        ]);
        chartPart.ChartSpace.Save();

        drawingsPart.WorksheetDrawing = BuildWorksheetDrawing(drawingsPart.GetIdOfPart(chartPart));
        drawingsPart.WorksheetDrawing.Save();

        var worksheet = sheetPart.Worksheet;
        var drawingElement = new Ss.Drawing { Id = sheetPart.GetIdOfPart(drawingsPart) };

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

    private static Xdr.WorksheetDrawing BuildWorksheetDrawing(string chartRelationshipId)
    {
        var fromMarker = new Xdr.FromMarker(
            new Xdr.ColumnId("0"), new Xdr.ColumnOffset("0"),
            new Xdr.RowId("2"), new Xdr.RowOffset("0"));

        var toMarker = new Xdr.ToMarker(
            new Xdr.ColumnId("4"), new Xdr.ColumnOffset("0"),
            new Xdr.RowId("14"), new Xdr.RowOffset("0"));

        var graphicFrame = new Xdr.GraphicFrame(
            new Xdr.NonVisualGraphicFrameProperties(
                new Xdr.NonVisualDrawingProperties { Id = 1, Name = "New Removed Chart" },
                new Xdr.NonVisualGraphicFrameDrawingProperties()),
            new Xdr.Transform(new A.Offset { X = 0, Y = 0 }, new A.Extents { Cx = 0, Cy = 0 }),
            new A.Graphic(new A.GraphicData(
                new C.ChartReference { Id = chartRelationshipId })
            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" }));

        var anchor = new Xdr.TwoCellAnchor(fromMarker, toMarker, graphicFrame, new Xdr.ClientData());

        return new Xdr.WorksheetDrawing(anchor);
    }
}
