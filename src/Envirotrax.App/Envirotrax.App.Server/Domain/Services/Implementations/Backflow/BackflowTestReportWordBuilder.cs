using System.Globalization;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using MiniSoftware;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowTestReportWordBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Word.Backflow.BackflowTestReport.docx";
    private const int BarMaxBlocks = 20;

    private static readonly Dictionary<string, string> CategoryColors = new()
    {
        ["Added By"] = "4682B4",
        ["Property Type"] = "20A845",
        ["Test Result"] = "207CE5",
        ["Reason for Test"] = "4682B4",
        ["Hazard Type"] = "207CE5",
        ["Assembly Type"] = "4682B4",
        ["Rain / Freeze Sensor"] = "20A845",
        ["On-site Sewage Facility"] = "20A845"
    };

    private const string PeriodsColor = "E87B20";
    private const string SubAccountsColor = "20A845";
    private const string DefaultCategoryColor = "20A845";

    public static byte[] Build(BackflowTestReportDto report, DateTime fromDate, DateTime toDate)
    {
        var templateBytes = ReadTemplateBytes();

        var value = new Dictionary<string, object>
        {
            ["dateRange"] = $"{fromDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)} - {toDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}",
            ["totalCount"] = report.TotalCount,
            ["hasSubAccounts"] = report.SubAccounts.Count > 0 ? "true" : "false",
            ["Periods"] = report.Periods
                .Select(p => new Dictionary<string, object>
                {
                    ["label"] = p.Label,
                    ["count"] = p.Count,
                    ["percentage"] = FormatPercentage(p.Percentage),
                    ["bar"] = BuildBar(p.Percentage, PeriodsColor)
                })
                .ToList(),
            ["SubAccounts"] = report.SubAccounts
                .Select(s => new Dictionary<string, object>
                {
                    ["name"] = s.Name,
                    ["count"] = s.Count,
                    ["percentage"] = FormatPercentage(s.Percentage),
                    ["bar"] = BuildBar(s.Percentage, SubAccountsColor)
                })
                .ToList(),
            ["Stats"] = report.Stats
                .Select(category =>
                {
                    var color = CategoryColors.GetValueOrDefault(category.Title, DefaultCategoryColor);

                    return new Dictionary<string, object>
                    {
                        ["Title"] = category.Title,
                        ["Items"] = category.Items
                            .Select(item => new Dictionary<string, object>
                            {
                                ["label"] = item.Label,
                                ["count"] = item.Count,
                                ["percentage"] = FormatPercentage(item.Percentage),
                                ["bar"] = BuildBar(item.Percentage, color)
                            })
                            .ToList()
                    };
                })
                .ToList()
        };

        using var output = new MemoryStream();
        MiniWord.SaveAsByTemplate(output, templateBytes, value);

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
            FontColor = hexColor
        };
    }

    private static byte[] ReadTemplateBytes()
    {
        using var templateStream = typeof(BackflowTestReportWordBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Word template embedded resource '{TemplateResourceName}' was not found.");

        using var buffer = new MemoryStream();
        templateStream.CopyTo(buffer);

        return buffer.ToArray();
    }
}
