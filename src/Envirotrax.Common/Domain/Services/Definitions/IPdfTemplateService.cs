namespace Envirotrax.Common.Domain.Services.Defintions;

public interface IPdfTemplateService
{
    Task<byte[]> GenerateAsync<T>(string pageName, T model);
}
