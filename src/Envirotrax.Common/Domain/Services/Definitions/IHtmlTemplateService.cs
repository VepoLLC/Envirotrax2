
using System.Dynamic;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IHtmlTemplateService
{
    Task<string> ParseEmailAsync<T>(string pageName, T model);

    Task<string> ParseEmailAsync<T>(string pageName, T model, ExpandoObject viewBag);
}