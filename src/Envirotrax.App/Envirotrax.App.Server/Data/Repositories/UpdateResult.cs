namespace Envirotrax.App.Server.Data.Repositories;

public class UpdateResult<TModel>
    where TModel : class
{
    public TModel? Model { get; set; }

    // Set by upsert-style repository methods (Add-or-Update in one call) to tell the caller which
    // RecordLogType applies: an insert has no field diff, so it needs a written "record added" log.
    public bool IsNew { get; set; }
}
