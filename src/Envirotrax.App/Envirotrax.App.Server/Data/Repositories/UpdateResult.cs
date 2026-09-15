namespace Envirotrax.App.Server.Data.Repositories;

public class UpdateResult<TModel>
    where TModel : class
{
    public TModel? Model { get; set; }

    public string Changes { get; set; } = string.Empty;

    // Set by upsert-style repository methods (Add-or-Update in one call) to tell the caller
    // which RecordLogType applies — Changes is never meaningful for a brand-new row.
    public bool IsNew { get; set; }
}
