using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IPdfTemplateService
{
    Task<byte[]> GenerateAsync<T>(string pageName, T model);

    IAsyncEnumerable<GeneratedPdfDto<TScriptResult>> GenerateManyAsync<T, TScriptResult>(string pageName, List<List<T>> documents, string resultScript);
}
