namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IPdfTemplateService
{
    Task<byte[]> GenerateAsync<T>(string pageName, T model);

    Task<List<byte[]>> GenerateManyAsync<T>(string pageName, List<T> models);
}
