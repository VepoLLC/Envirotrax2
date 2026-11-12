using System.Dynamic;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Options;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
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

    public async IAsyncEnumerable<GeneratedPdfDto<TScriptResult>> GenerateManyAsync<T, TScriptResult>(string pageName, List<List<T>> documents, string resultScript)
    {
        await using var browser = await LaunchBrowserAsync();

        foreach (var parts in documents)
        {
            var contents = new List<byte[]>();
            var scriptResults = new List<TScriptResult>();

            foreach (var part in parts)
            {
                await using var page = await OpenPageAsync(browser, pageName, part);

                scriptResults.Add(await page.EvaluateExpressionAsync<TScriptResult>(resultScript));
                contents.Add(await PrintPdfAsync(page));
            }

            yield return new GeneratedPdfDto<TScriptResult>
            {
                Content = MergePdfs(contents),
                ScriptResults = scriptResults
            };
        }
    }

    private static byte[] MergePdfs(List<byte[]> contents)
    {
        if (contents.Count == 1)
        {
            return contents[0];
        }

        using var mergedDocument = new PdfDocument();

        foreach (var content in contents)
        {
            using var partStream = new MemoryStream(content);
            using var partDocument = PdfReader.Open(partStream, PdfDocumentOpenMode.Import);

            foreach (var page in partDocument.Pages)
            {
                mergedDocument.AddPage(page);
            }
        }

        using var mergedStream = new MemoryStream();

        mergedDocument.Save(mergedStream);

        return mergedStream.ToArray();
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
        await using var page = await OpenPageAsync(browser, pageName, model);

        return await PrintPdfAsync(page);
    }

    private async Task<IPage> OpenPageAsync<T>(IBrowser browser, string pageName, T model)
    {
        dynamic viewBag = new ExpandoObject();

        viewBag.BaseUrl = _options.BaseUrl;

        string html = await _htmlTemplateService.ParsePdfAsync(pageName, model, viewBag);

        var page = await browser.NewPageAsync();

        await page.SetContentAsync(html, new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Networkidle0]
        });

        return page;
    }

    private static Task<byte[]> PrintPdfAsync(IPage page)
    {
        return page.PdfDataAsync(new PdfOptions
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
