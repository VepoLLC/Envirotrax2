
using System.Collections.Specialized;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.Common.Domain.Services.Implementations;

public class QueryHelperService : IQueryHelperService
{
    public NameValueCollection BuildQuery(PageInfo pageInfo, Query query)
    {
        var collection = new NameValueCollection
        {
            { "pageNumber", (pageInfo.PageNumber > 0 ? pageInfo.PageNumber : 1).ToString() },
            { "pageSize", (pageInfo.PageSize > 0 ? pageInfo.PageSize : 20).ToString() }
        };

        if (query.Sort != null)
        {
            foreach (var kvp in query.Sort)
            {
                collection.Add($"s[{kvp.Key}]", kvp.Value.ToString());
            }
        }

        SetQuery(collection, query.Filter, parentPath: null);

        return collection;
    }

    private void SetQuery(NameValueCollection collection, List<QueryProperty>? filter, string? parentPath)
    {
        if (filter == null) return;

        for (int i = 0; i < filter.Count; i++)
        {
            var property = filter[i];
            var itemPath = parentPath != null ? $"{parentPath}.q[{i}]" : $"q[{i}]";

            collection.Add($"{itemPath}.col", property.ColumnName ?? string.Empty);

            if (property.Value != null)
            {
                collection.Add($"{itemPath}.val", property.Value.ToString());
            }

            collection.Add($"{itemPath}.lop", property.LogicalOperator.ToString());
            collection.Add($"{itemPath}.op", property.ComparisonOperator.ToString());

            if (property.IsValueNull)
            {
                collection.Add($"{itemPath}.null", "true");
            }

            SetQuery(collection, property.Children, itemPath);
        }
    }
}
