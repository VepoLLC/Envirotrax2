using System.Dynamic;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class PdfTemplateService : IPdfTemplateService
{
    private static readonly SemaphoreSlim _downloadLock = new(1, 1);
    private static bool _chromiumReady;

    private readonly IHtmlTemplateService _htmlTemplateService;
    private readonly PdfTemplateOptions _options;

    public PdfTemplateService(IHtmlTemplateService htmlTemplateService, IOptions<PdfTemplateOptions> options)
    {
        _htmlTemplateService = htmlTemplateService;
        _options = options.Value;
    }

    public async Task<byte[]> GenerateAsync<T>(string pageName, T model)
    {
        await using var browser = await LaunchBrowserAsync();

        return await GeneratePdfAsync(browser, pageName, model);
    }

    public async Task<List<byte[]>> GenerateManyAsync<T>(string pageName, List<T> models)
    {
        var pdfs = new List<byte[]>();

        await using var browser = await LaunchBrowserAsync();

        foreach (var model in models)
        {
            var pdf = await GeneratePdfAsync(browser, pageName, model);

            pdfs.Add(pdf);
        }

        return pdfs;
    }

    private async Task<IBrowser> LaunchBrowserAsync()
    {
        var executablePath = await ResolveChromiumPathAsync();

        return await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = executablePath,
            Args = ["--no-sandbox", "--disable-setuid-sandbox"]
        });
    }

    private async Task<byte[]> GeneratePdfAsync<T>(IBrowser browser, string pageName, T model)
    {
        dynamic viewBag = new ExpandoObject();

        viewBag.BaseUrl = _options.BaseUrl;

        string html = await _htmlTemplateService.ParsePdfAsync(pageName, model, viewBag);

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

    private async Task<string?> ResolveChromiumPathAsync()
    {
        if (!_options.IsDevelopment)
        {
            if (string.IsNullOrEmpty(_options.ChromiumExecutablePath))
            {
                throw new InvalidOperationException("PdfTemplate:ChromiumExecutablePath must be configured in non-development environments.");
            }

            return _options.ChromiumExecutablePath;
        }

        if (_chromiumReady)
        {
            return null;
        }

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

        return null;
    }
}
