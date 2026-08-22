using DocumentFormat.OpenXml.Packaging;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using MiniSoftware;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public static class BackflowNewRemovedWordBuilder
{
    private const string TemplateResourceName = "Envirotrax.App.Server.Templates.Word.Backflow.BackflowNewRemoved.docx";
    private const string CreatedColor = "F0922B";
    private const string RemovedColor = "5AC8E8";

    public static byte[] Build(BackflowNewRemovedReportDto report)
    {
        var templateBytes = ReadTemplateBytes();

        using var output = new MemoryStream();
        MiniWord.SaveAsByTemplate(output, templateBytes, new Dictionary<string, object>());

        AddChart(output, report.Points);

        return output.ToArray();
    }

    // MiniWord has no chart support, so — mirroring BackflowComplianceHistoryWordBuilder — the chart is
    // injected as a native OOXML DrawingML chart part, replacing the literal "{{barChart}}" placeholder
    // the template reserves for it (MiniWord only rewrites recognized {{tag}} placeholders, so this
    // unrecognized one survives template processing untouched).
    private static void AddChart(MemoryStream documentStream, IReadOnlyList<BackflowNewRemovedPointDto> points)
    {
        documentStream.Position = 0;

        using (var doc = WordprocessingDocument.Open(documentStream, true))
        {
            var mainPart = doc.MainDocumentPart!;

            if (points.Count > 0)
            {
                var categories = points.Select(p => p.Label).ToList();

                BackflowWordChartEmbedder.EmbedChart(mainPart, "{{barChart}}", "New Removed Chart", docPrId: 1, BackflowGroupedBarChartHelper.BuildChartSpace(categories,
                [
                    ("Assemblies Created", CreatedColor, points.Select(p => p.Created).ToList()),
                    ("Assemblies Removed", RemovedColor, points.Select(p => p.Removed).ToList())
                ]));
            }

            MiniWordDocumentFixup.FixColoredTextRunProperties(mainPart);
            mainPart.Document.Save();
        }

        documentStream.Position = 0;
    }

    private static byte[] ReadTemplateBytes()
    {
        using var templateStream = typeof(BackflowNewRemovedWordBuilder).Assembly.GetManifestResourceStream(TemplateResourceName)
            ?? throw new InvalidOperationException($"Word template embedded resource '{TemplateResourceName}' was not found.");

        using var buffer = new MemoryStream();
        templateStream.CopyTo(buffer);

        return buffer.ToArray();
    }
}
