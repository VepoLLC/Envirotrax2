using DeveloperPartners.SortingFiltering;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public static class ProfessionalRecordScope
{
    private const string WaterSupplierColumn = "WaterSupplierId";

    public static void ApplyToProfessionalSearch(Query query, int professionalId, string professionalColumnName)
    {
        if (professionalId <= 0 || HasWaterSupplierCriteria(query))
        {
            return;
        }

        query.Filter.Add(new QueryProperty
        {
            ColumnName = professionalColumnName,
            Value = professionalId.ToString(),
            LogicalOperator = LogicalOperator.And
        });
    }

    private static bool HasWaterSupplierCriteria(Query query)
    {
        foreach (var filter in query.Filter)
        {
            var columnName = filter.ColumnName?.Replace(".", string.Empty);

            if (string.Equals(columnName, WaterSupplierColumn, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
