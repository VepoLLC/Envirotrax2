
namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IHtmlTemplateService
{
    Task<string> ParseEmailAsync<T>(string pageName, T model);
}