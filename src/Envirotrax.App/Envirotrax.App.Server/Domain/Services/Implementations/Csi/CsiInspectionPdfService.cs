using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionPdfService : ICsiInspectionPdfService
{
    private readonly IRazorRenderService _razorRenderer;

    public CsiInspectionPdfService(IRazorRenderService razorRenderer)
    {
        _razorRenderer = razorRenderer;
    }

    public async Task<byte[]> GenerateAsync(CsiInspectionDto inspection)
    {
        var html = await _razorRenderer.RenderAsync("/Views/PdfTemplates/Csi/CsiInspection.cshtml", inspection);

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-setuid-sandbox"]
        });

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Networkidle0]
        });

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.Letter,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "1.5cm", Bottom = "1.5cm",
                Left = "1.5cm", Right = "1.5cm"
            }
        });
    }
}
