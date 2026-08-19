using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using A = DocumentFormat.OpenXml.Drawing;
using C = DocumentFormat.OpenXml.Drawing.Charts;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Ss = DocumentFormat.OpenXml.Spreadsheet;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowComplianceReportExcelBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Excel.Backflow.BackflowComplianceReport.xlsx";
    private const string BarColor = "#20A845";
    private const string TotalColor = "#ADB5BD";
    private const string CompliantColor = "#20A845";
    private const string NonCompliantColor = "#DC3545";

    public static byte[] Build(BackflowComplianceReportDto report, bool ignoreLast30Days)
    {
        using var templateStream = typeof(BackflowComplianceReportExcelBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Excel template embedded resource '{TemplateResourceName}' was not found.");

        using var workbook = new XLWorkbook(templateStream);
        var ws = workbook.Worksheet("Report");
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;

        if (ignoreLast30Days)
        {
            ws.Cell(1, 1).Value = "Backflow Compliance Report (Ignoring Last 30 Days)";
        }

        WriteLegendRow(ws, row: 5, TotalColor, "Total Active Assemblies:", report.TotalActive, percentage: null);
        WriteLegendRow(ws, row: 6, CompliantColor, "Compliant:", report.Compliant, report.CompliantPercentage);
        WriteLegendRow(ws, row: 7, NonCompliantColor, "Non-Compliant:", report.NonCompliant, report.NonCompliantPercentage);

        WriteRequirements(ws, headerRow: 11, report.Requirements);

        using var output = new MemoryStream();
        workbook.SaveAs(output);

        AddComplianceChart(output, "Report", report.Compliant, report.NonCompliant);

        return output.ToArray();
    }

    // A legend row is a colored swatch (column E) followed by a label spanning F:G, a count (H), and
    // an optional percentage (I) — matching the doughnut's legend in the PDF/on-screen report.
    private static void WriteLegendRow(IXLWorksheet ws, int row, string swatchColor, string label, int count, double? percentage)
    {
        ws.Cell(row, 5).Style.Fill.BackgroundColor = XLColor.FromHtml(swatchColor);

        var labelRange = ws.Range(row, 6, row, 7);
        labelRange.Merge();
        labelRange.FirstCell().Value = label;
        labelRange.FirstCell().Style.Font.Bold = true;

        ws.Cell(row, 8).Value = count;

        if (percentage.HasValue)
        {
            ws.Cell(row, 9).Value = $"{percentage.Value:0}%";
        }
    }

    // The "Requirements" section is a section-header row, a column-header row, and a template data row.
    // Deletes the whole section (all 3 rows) when there are no requirements.
    private static void WriteRequirements(IXLWorksheet ws, int headerRow, IReadOnlyList<BackflowComplianceRequirementDto> requirements)
    {
        var columnHeaderRow = headerRow + 1;
        var templateRow = headerRow + 2;

        if (requirements.Count == 0)
        {
            ws.Row(headerRow).Delete();
            ws.Row(headerRow).Delete();
            ws.Row(headerRow).Delete();

            return;
        }

        CloneRowsBelow(ws, templateRow, requirements.Count - 1);

        for (var i = 0; i < requirements.Count; i++)
        {
            var requirement = requirements[i];
            var row = templateRow + i;

            ws.Cell(row, 1).Value = requirement.PropertyType;
            ws.Cell(row, 2).Value = requirement.AssemblyType;
            ws.Cell(row, 3).Value = requirement.HazardType;
            ws.Cell(row, 4).Value = requirement.HasSiteOssf ? "1" : "0";
            ws.Cell(row, 5).Value = requirement.AuxWaterSupply ? "1" : "0";
            ws.Cell(row, 6).Value = requirement.RenewalYears;

            ws.Cell(row, 7).Value = requirement.Percentage;
            ws.Cell(row, 7).Style.NumberFormat.Format = ";;;";

            ws.Cell(row, 8).Value = requirement.Active;
            ws.Cell(row, 9).Value = requirement.Compliant;

            ws.Cell(row, 10).Value = requirement.Percentage;
            ws.Cell(row, 10).Style.NumberFormat.Format = "0\"%\"";
        }

        var range = ws.Range(templateRow, 7, templateRow + requirements.Count - 1, 7);
        range.AddConditionalFormat().DataBar(XLColor.FromHtml(BarColor));
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

    // ClosedXML (0.105.0) doesn't expose a public API for creating charts, so the doughnut chart is
    // injected as a native OOXML DrawingML chart part directly into the workbook ClosedXML already
    // produced. It lands in the blank rows 5-18 reserved for it in the template, to the right of the
    // summary lines and above the "Requirements" section.
    private static void AddComplianceChart(MemoryStream workbookStream, string sheetName, int compliant, int nonCompliant)
    {
        workbookStream.Position = 0;

        using var doc = SpreadsheetDocument.Open(workbookStream, true);
        var workbookPart = doc.WorkbookPart!;
        var sheet = workbookPart.Workbook.Descendants<Ss.Sheet>().First(s => s.Name == sheetName);
        var sheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);

        var drawingsPart = sheetPart.AddNewPart<DrawingsPart>();
        var chartPart = drawingsPart.AddNewPart<ChartPart>();

        chartPart.ChartSpace = BackflowComplianceChartHelper.BuildChartSpace(compliant, nonCompliant, includeLegend: false);
        chartPart.ChartSpace.Save();

        drawingsPart.WorksheetDrawing = BuildWorksheetDrawing(drawingsPart.GetIdOfPart(chartPart));
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

    private static Xdr.WorksheetDrawing BuildWorksheetDrawing(string chartRelationshipId)
    {
        // Anchored in the blank rows/columns reserved for it: columns A-D, rows 2-10 (0-based markers),
        // to the left of the summary legend in columns G-J.
        var fromMarker = new Xdr.FromMarker(
            new Xdr.ColumnId("0"), new Xdr.ColumnOffset("0"),
            new Xdr.RowId("1"), new Xdr.RowOffset("0"));

        var toMarker = new Xdr.ToMarker(
            new Xdr.ColumnId("4"), new Xdr.ColumnOffset("0"),
            new Xdr.RowId("10"), new Xdr.RowOffset("0"));

        var graphicFrame = new Xdr.GraphicFrame(
            new Xdr.NonVisualGraphicFrameProperties(
                new Xdr.NonVisualDrawingProperties { Id = 1, Name = "Compliance Chart" },
                new Xdr.NonVisualGraphicFrameDrawingProperties()),
            new Xdr.Transform(new A.Offset { X = 0, Y = 0 }, new A.Extents { Cx = 0, Cy = 0 }),
            new A.Graphic(new A.GraphicData(
                new C.ChartReference { Id = chartRelationshipId })
            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" }));

        var anchor = new Xdr.TwoCellAnchor(fromMarker, toMarker, graphicFrame, new Xdr.ClientData());

        return new Xdr.WorksheetDrawing(anchor);
    }
}
