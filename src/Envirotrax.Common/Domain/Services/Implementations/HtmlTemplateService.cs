
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

        public HtmlTemplateService(IOptions<HtmlTemplateOptions> templateOptions)
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(templateOptions.Value.Assembly, templateOptions.Value.Namespace)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> ParseEmailAsync<T>(string pageName, T model)
        {
            return await ParseStringAsync($"Emails.{pageName}", model, null);
        }

        public async Task<string> ParseEmailAsync<T>(string pageName, T model, ExpandoObject viewBag)
        {
            return await ParseStringAsync($"Emails.{pageName}", model, viewBag);
        }

        private async Task<string> ParseStringAsync<T>(string pageName, T model, ExpandoObject? viewBag)
        {
            return await _engine.CompileRenderAsync($"Templates.{pageName}", model, viewBag: viewBag);
        }
    }
}