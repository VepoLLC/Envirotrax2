using Envirotrax.Common.Domain.Services.Defintions;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class PdfTemplateService : IPdfTemplateService
{
    private static readonly SemaphoreSlim _downloadLock = new(1, 1);
    private static bool _chromiumReady;

    private readonly IHtmlTemplateService _htmlTemplateService;

    public PdfTemplateService(IHtmlTemplateService htmlTemplateService)
    {
        _htmlTemplateService = htmlTemplateService;
    }

    public async Task<byte[]> GenerateAsync<T>(string pageName, T model)
    {
        await EnsureChromiumAsync();

        var html = await _htmlTemplateService.ParseStringAsync($"Pdf.{pageName}", model, null);

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
                Top = "1.5cm",
                Bottom = "1.5cm",
                Left = "1.5cm",
                Right = "1.5cm"
            }
        });
    }

    private static async Task EnsureChromiumAsync()
    {
        if (_chromiumReady) return;

        await _downloadLock.WaitAsync();
        try
        {
            if (!_chromiumReady)
            {
                await new BrowserFetcher().DownloadAsync();
                _chromiumReady = true;
            }
        }
        finally
        {
            _downloadLock.Release();
        }
    }
}
