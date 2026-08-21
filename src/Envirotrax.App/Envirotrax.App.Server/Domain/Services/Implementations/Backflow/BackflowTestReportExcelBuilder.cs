using System.Globalization;
using ClosedXML.Excel;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowTestReportExcelBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Excel.Backflow.BackflowTestReport.xlsx";

    private static readonly Dictionary<string, string> CategoryColors = new()
    {
        ["Added By"] = "#4682B4",
        ["Property Type"] = "#20A845",
        ["Test Result"] = "#207CE5",
        ["Reason for Test"] = "#4682B4",
        ["Hazard Type"] = "#207CE5",
        ["Assembly Type"] = "#4682B4",
        ["Rain / Freeze Sensor"] = "#20A845",
        ["On-site Sewage Facility"] = "#20A845"
    };

    private const string PeriodsColor = "#E87B20";
    private const string SubAccountsColor = "#20A845";
    private const string DefaultCategoryColor = "#20A845";

    public static byte[] Build(BackflowTestReportDto report, DateTime fromDate, DateTime toDate)
    {
        using var templateStream = typeof(BackflowTestReportExcelBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Excel template embedded resource '{TemplateResourceName}' was not found.");

        using var workbook = new XLWorkbook(templateStream);
        var ws = workbook.Worksheet("Report");

        ws.Cell(2, 2).Value = $"{fromDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)} - {toDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}";
        ws.Cell(3, 2).Value = report.TotalCount;

        var cursor = 4;

        cursor = WriteListSection(ws, cursor, report.Periods.Count,
            fill: (row, i) =>
            {
                var period = report.Periods[i];
                ws.Cell(row, 1).Value = period.Label;
                FillBarCountPercentage(ws, row, period.Count, period.Percentage);
            },
            barColor: PeriodsColor);

        var statsTotalRow = cursor + 1;
        ws.Cell(statsTotalRow, 2).Value = report.TotalCount;
        cursor = statsTotalRow + 1;

        cursor = WriteListSection(ws, cursor, report.SubAccounts.Count,
            fill: (row, i) =>
            {
                var sub = report.SubAccounts[i];
                ws.Cell(row, 1).Value = sub.Name;
                FillBarCountPercentage(ws, row, sub.Count, sub.Percentage);
            },
            barColor: SubAccountsColor);

        WriteCategoryBlocks(ws, cursor, report.Stats);

        using var output = new MemoryStream();
        workbook.SaveAs(output);

        return output.ToArray();
    }

    // A "section" is a header row followed by a template item row. Writes `itemCount` items starting
    // at the template row (cloning it as needed), or deletes the header+template row pair when empty.
    // Returns the row number immediately after the section.
    private static int WriteListSection(IXLWorksheet ws, int headerRow, int itemCount, Action<int, int> fill, string barColor)
    {
        var templateRow = headerRow + 1;

        if (itemCount == 0)
        {
            ws.Row(headerRow).Delete();
            ws.Row(headerRow).Delete();

            return headerRow;
        }

        CloneRowsBelow(ws, templateRow, itemCount - 1);

        for (var i = 0; i < itemCount; i++)
        {
            fill(templateRow + i, i);
        }

        ApplyDataBar(ws, templateRow, itemCount, column: 2, barColor);

        return templateRow + itemCount;
    }

    private static void WriteCategoryBlocks(IXLWorksheet ws, int blockHeaderRow, IReadOnlyList<BackflowReportStatCategoryDto> categories)
    {
        if (categories.Count == 0)
        {
            ws.Row(blockHeaderRow).Delete();
            ws.Row(blockHeaderRow).Delete();

            return;
        }

        // Clone the master header+item block as a flat unit for every category after the first,
        // so each category temporarily has exactly one item row before per-category expansion below.
        var templateItemRow = blockHeaderRow + 1;

        for (var i = 1; i < categories.Count; i++)
        {
            var headerSource = ws.Row(blockHeaderRow);
            var itemSource = ws.Row(templateItemRow);
            var inserted = itemSource.InsertRowsBelow(2).ToList();

            headerSource.CopyTo(inserted[0]);
            itemSource.CopyTo(inserted[1]);
        }

        var currentHeaderRow = blockHeaderRow;

        foreach (var category in categories)
        {
            ws.Cell(currentHeaderRow, 1).Value = category.Title;

            var color = CategoryColors.GetValueOrDefault(category.Title, DefaultCategoryColor);
            var itemTemplateRow = currentHeaderRow + 1;

            CloneRowsBelow(ws, itemTemplateRow, category.Items.Count - 1);

            for (var i = 0; i < category.Items.Count; i++)
            {
                var item = category.Items[i];
                var row = itemTemplateRow + i;

                ws.Cell(row, 1).Value = item.Label;
                FillBarCountPercentage(ws, row, item.Count, item.Percentage);
            }

            if (category.Items.Count == 0)
            {
                ws.Row(itemTemplateRow).Delete();
            }
            else
            {
                ApplyDataBar(ws, itemTemplateRow, category.Items.Count, column: 2, color);
            }

            currentHeaderRow = itemTemplateRow + category.Items.Count;
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

    private static void ApplyDataBar(IXLWorksheet ws, int startRow, int rowCount, int column, string color)
    {
        var range = ws.Range(startRow, column, startRow + rowCount - 1, column);
        range.AddConditionalFormat().DataBar(XLColor.FromHtml(color));
    }

    // Column 2 carries only the data bar (its value drives the bar's fill but the number itself is
    // hidden via a blank number format); column 3 is the plain count; column 4 is the visible percentage.
    private static void FillBarCountPercentage(IXLWorksheet ws, int row, int count, double percentage)
    {
        ws.Cell(row, 2).Value = percentage;
        ws.Cell(row, 2).Style.NumberFormat.Format = ";;;";

        ws.Cell(row, 3).Value = count;

        ws.Cell(row, 4).Value = percentage;
        ws.Cell(row, 4).Style.NumberFormat.Format = "0\"%\"";
    }
}
