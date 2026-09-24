using System.Collections.Concurrent;
using System.Globalization;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Envirotrax.App.Server.Data.Models.Logs;

/// <summary>
/// The per-entity record logging rules, read once from <see cref="RecordLoggedAttribute"/> and cached.
/// </summary>
public sealed class RecordLogDescriptor
{
    private static readonly ConcurrentDictionary<Type, RecordLogDescriptor?> Descriptors = new();

    private readonly RecordLoggedAttribute _attribute;
    private readonly string _recordIdProperty;
    private readonly string? _waterSupplierIdProperty;
    private readonly string? _professionalIdProperty;

    private RecordLogDescriptor(RecordLoggedAttribute attribute, IEntityType entityType)
    {
        _attribute = attribute;

        _recordIdProperty = attribute.RecordIdProperty ?? AppEntityKeys.GetPrimaryKeyName(entityType);

        _waterSupplierIdProperty = FindProperty(entityType, attribute.WaterSupplierSource, attribute.WaterSupplierIdProperty ?? nameof(RecordLog.WaterSupplierId));
        _professionalIdProperty = FindProperty(entityType, attribute.ProfessionalSource, nameof(RecordLog.ProfessionalId));
    }

    /// <summary>
    /// Returns the logging rules for an entity type, or null when the entity is not record logged.
    /// </summary>
    public static RecordLogDescriptor? Resolve(IEntityType entityType)
    {
        return Descriptors.GetOrAdd(entityType.ClrType, _ =>
        {
            var attribute = entityType.ClrType.GetCustomAttributes(typeof(RecordLoggedAttribute), inherit: false)
                .Cast<RecordLoggedAttribute>()
                .FirstOrDefault();

            return attribute == null
                ? null
                : new RecordLogDescriptor(attribute, entityType);
        });
    }

    public RecordLog CreateLog(EntityEntry entry, ITenantProvidersService tenantProvider, RecordLogType logType, string description)
    {
        return new RecordLog
        {
            LogType = logType,
            TableName = _attribute.TableName,
            RecordId = GetRecordId(entry),
            Description = description,
            WaterSupplierId = GetId(entry, _attribute.WaterSupplierSource, _waterSupplierIdProperty, tenantProvider.WaterSupplierId),
            ProfessionalId = GetId(entry, _attribute.ProfessionalSource, _professionalIdProperty, tenantProvider.ProfessionalId),
            IpAddress = tenantProvider.IpAddress
        };
    }

    private int GetRecordId(EntityEntry entry)
    {
        var value = entry.Property(_recordIdProperty).CurrentValue;

        return value == null
            ? 0
            : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static int? GetId(EntityEntry entry, RecordLogIdSource source, string? propertyName, int ambientId)
    {
        var id = source switch
        {
            RecordLogIdSource.Entity when propertyName != null => entry.Property(propertyName).CurrentValue as int?,
            RecordLogIdSource.Ambient => ambientId,
            _ => null
        };

        // The tenant provider reports "none" as 0, and every RecordLog foreign key is Restrict,
        // so 0 has to become null rather than a reference to a row that does not exist.
        return id > 0 ? id : null;
    }

    private static string? FindProperty(IEntityType entityType, RecordLogIdSource source, string propertyName)
    {
        if (source != RecordLogIdSource.Entity)
        {
            return null;
        }

        return entityType.FindProperty(propertyName) == null
            ? null
            : propertyName;
    }
}
