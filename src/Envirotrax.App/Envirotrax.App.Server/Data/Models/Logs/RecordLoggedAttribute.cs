namespace Envirotrax.App.Server.Data.Models.Logs;

/// <summary>
/// Marks an entity as one whose edits can be written to RecordLogs automatically by
/// <c>TenantDbContext.SaveChangesAndLogAsync</c>. Entities without this attribute are never
/// logged automatically, so unrelated rows caught up in the same SaveChanges stay out of the log.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RecordLoggedAttribute : Attribute
{
    public RecordLoggedAttribute(string tableName)
    {
        TableName = tableName;
    }

    /// <summary>
    /// The value written to RecordLog.TableName — a <see cref="RecordLogTableNames"/> constant.
    /// These are V1-compatible names and do not always match the database table, so they are
    /// always stated explicitly rather than inferred.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Property whose value becomes RecordLog.RecordId. Defaults to the entity's non-shadow
    /// AppPrimaryKey column, which is what the hand-written call sites already use.
    /// </summary>
    public string? RecordIdProperty { get; set; }

    /// <summary>
    /// Where RecordLog.WaterSupplierId comes from. The default reads it off the entity and falls
    /// back to null for entities that have no such column (professional-owned records).
    /// </summary>
    public RecordLogIdSource WaterSupplierSource { get; set; } = RecordLogIdSource.Entity;

    /// <summary>
    /// Property the Entity source reads the water supplier id from. Defaults to WaterSupplierId;
    /// set it for entities that carry the id under another name (WaterSupplier's own Id).
    /// </summary>
    public string? WaterSupplierIdProperty { get; set; }

    /// <summary>
    /// Where RecordLog.ProfessionalId comes from. The default is the acting professional, which is
    /// what services pass today (and resolves to null on water supplier and admin paths).
    /// </summary>
    public RecordLogIdSource ProfessionalSource { get; set; } = RecordLogIdSource.Ambient;
}

public enum RecordLogIdSource
{
    /// <summary>Read the id off the entity being logged; null when it has no such property.</summary>
    Entity,

    /// <summary>Take the id of the caller from the current request.</summary>
    Ambient,

    /// <summary>Always null.</summary>
    None
}
