using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// ClosedXML (0.105.0) writes DataBar's Excel-2010-extension linkage ids — the legacy cfRule's `id`
// attribute, the worksheet-level x14:cfRule's `id` attribute, and the `<x14:id>` element that links
// them — as lowercase-hex GUIDs. OOXML's ST_Guid base type only permits uppercase A-F in that
// pattern, so Excel's strict parser rejects the file and shows a "We found a problem... recover?"
// prompt on open (the data itself is unaffected — Excel silently accepts the file once told to
// recover). Confirmed via OpenXmlValidator across every Backflow report that uses
// AddConditionalFormat().DataBar(...) (compliance report/history/test report) and confirmed
// independent of chart injection or row cloning — DataBar alone, with no chart-injection code
// anywhere nearby, reproduces the identical two validation errors. ClosedXML has no public API to
// control this casing, so this fixes it directly in the saved worksheet XML.
internal static class BackflowDataBarGuidFixup
{
    private static readonly Regex WorksheetPartName = new(@"^xl/worksheets/sheet\d+\.xml$", RegexOptions.Compiled);

    private static readonly Regex IdAttributePattern = new(
        @"(?<prefix>\bid="")(?<guid>\{[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\})""",
        RegexOptions.Compiled);

    private static readonly Regex IdElementPattern = new(
        @"(?<open><x14:id>)(?<guid>\{[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\})(?<close></x14:id>)",
        RegexOptions.Compiled);

    public static void FixWorksheetParts(MemoryStream workbookStream)
    {
        workbookStream.Position = 0;

        using (var archive = new ZipArchive(workbookStream, ZipArchiveMode.Update, leaveOpen: true))
        {
            foreach (var entry in archive.Entries.Where(e => WorksheetPartName.IsMatch(e.FullName)).ToList())
            {
                string original;

                using (var reader = new StreamReader(entry.Open(), Encoding.UTF8))
                {
                    original = reader.ReadToEnd();
                }

                var fixedXml = UppercaseDataBarIds(original);

                if (fixedXml == original)
                {
                    continue;
                }

                using var entryStream = entry.Open();
                entryStream.SetLength(0);

                using var writer = new StreamWriter(entryStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                writer.Write(fixedXml);
            }
        }

        workbookStream.Position = 0;
    }

    private static string UppercaseDataBarIds(string xml)
    {
        var fixedXml = IdAttributePattern.Replace(xml, m => m.Groups["prefix"].Value + m.Groups["guid"].Value.ToUpperInvariant() + "\"");

        return IdElementPattern.Replace(fixedXml, m => m.Groups["open"].Value + m.Groups["guid"].Value.ToUpperInvariant() + m.Groups["close"].Value);
    }
}
