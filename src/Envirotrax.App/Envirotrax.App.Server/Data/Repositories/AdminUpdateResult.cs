namespace Envirotrax.App.Server.Data.Repositories;

public class AdminUpdateResult<TModel>
    where TModel : class
{
    public TModel? Model { get; set; }

    public string Changes { get; set; } = string.Empty;
}
