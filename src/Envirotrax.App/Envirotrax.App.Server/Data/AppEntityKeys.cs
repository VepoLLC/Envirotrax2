using System.Reflection;
using Envirotrax.Common.Data.Attributes;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Envirotrax.App.Server.Data;

public static class AppEntityKeys
{
    /// <summary>
    /// The key property rows are addressed by. Our db primary key names differ per table, and tenant
    /// models carry a composite key where only one part is the real (non-shadow) key — this returns
    /// that one, which is also the id the record logs are written against.
    /// </summary>
    public static string GetPrimaryKeyName(IEntityType entityType)
    {
        var primaryKey = entityType.FindPrimaryKey()!;

        var property = primaryKey.Properties
            .FirstOrDefault(p =>
                p.PropertyInfo?.GetCustomAttribute<AppPrimaryKeyAttribute>()?.IsShadowKey == false);

        return property?.Name
               ?? primaryKey.Properties.First().Name;
    }
}
