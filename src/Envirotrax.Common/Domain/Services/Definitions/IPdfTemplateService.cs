using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IPdfTemplateService
{
    Task<byte[]> GenerateAsync<T>(string pageName, T model);

    Task<List<GeneratedPdfDto<TScriptResult>>> GenerateManyAsync<T, TScriptResult>(string pageName, List<T> models, string resultScript);
}
