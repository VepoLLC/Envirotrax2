
using System.Dynamic;
using System.Reflection;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Options;
using RazorLight;

namespace Envirotrax.Common.Domain.Services.Implementations
{
    public class HtmlTemplateService : IHtmlTemplateService
    {
        private readonly RazorLightEngine _engine;
        private readonly string _baseUrl;

        public HtmlTemplateService(IOptions<HtmlTemplateOptions> templateOptions)
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(templateOptions.Value.Assembly, templateOptions.Value.Namespace)
                .UseMemoryCachingProvider()
                .Build();

            _baseUrl = templateOptions.Value.BaseUrl;
        }

        public async Task<string> ParseEmailAsync<T>(string pageName, T model)
        {
            return await ParseStringAsync($"Emails.{pageName}", model);
        }

        public async Task<string> ParsePdfAsync<T>(string pageName, T model)
        {
            return await ParseStringAsync($"Pdf.{pageName}", model);
        }

        private async Task<string> ParseStringAsync<T>(string pageName, T model)
        {
            var viewBag = new ExpandoObject();
            viewBag.TryAdd("BaseUrl", _baseUrl);

            return await _engine.CompileRenderAsync($"Templates.{pageName}", model, viewBag: viewBag);
        }
    }
}