namespace Envirotrax.App.Server.Domain.Services.Definitions;

public interface IRazorRenderService
{
    Task<string> RenderAsync<TModel>(string viewName, TModel model);
}
