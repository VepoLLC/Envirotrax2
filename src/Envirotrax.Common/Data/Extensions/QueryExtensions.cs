using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Common.Data.Extensions;

public static class QueryExtensions
{
    /// <summary>
    /// Adds a "DeletedTime IS NULL" condition so the query leaves soft-deleted rows out.
    /// </summary>
    /// <remarks>
    /// Queries that are model-bound from a request already get this from the QueryFilter action filter.
    /// This is for the queries we build in code — counts, background jobs — which would otherwise
    /// include soft-deleted rows. A query that already filters on DeletedTime is returned untouched,
    /// so callers that deliberately ask for deleted rows keep working.
    /// </remarks>
    public static Query ExcludeDeleted(this Query query)
    {
        var existingFilter = query.Filter.Find(f => f.ColumnName == nameof(IDeleteAutitableModel<>.DeletedTime));

        if (existingFilter != null)
        {
            return query;
        }

        query.Filter.Add(new QueryProperty
        {
            ColumnName = nameof(IDeleteAutitableModel<>.DeletedTime),
            IsValueNull = true,
            LogicalOperator = LogicalOperator.And
        });

        return query;
    }
}
