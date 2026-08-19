using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

// MiniWord 0.9.2's internal colored-text substitution (used for our bar visuals) produces a few
// schema-invalid <w:rPr> shapes: <w:color>/<w:shd> emitted in the wrong CT_RPr order, <w:shd> missing
// its required Val attribute, and rPr left after a stray leftover empty <w:t> instead of first in the
// run. Word tolerates this silently in practice, but it's easy to correct once the document is open
// for other reasons (chart injection), so every MiniWord-produced document gets patched before returning.
internal static class MiniWordDocumentFixup
{
    public static void FixColoredTextRunProperties(MainDocumentPart mainPart)
    {
        foreach (var rPr in mainPart.Document.Descendants<RunProperties>().ToList())
        {
            var color = rPr.GetFirstChild<Color>();
            var shading = rPr.GetFirstChild<Shading>();

            if (color != null && shading != null && rPr.Elements().ToList().IndexOf(shading) < rPr.Elements().ToList().IndexOf(color))
            {
                color.Remove();
                rPr.InsertBefore(color, shading);
            }

            if (shading != null && shading.Val == null)
            {
                shading.Val = ShadingPatternValues.Clear;
            }

            if (rPr.Parent is Run run && run.FirstChild != rPr)
            {
                rPr.Remove();
                run.PrependChild(rPr);
            }
        }
    }
}
