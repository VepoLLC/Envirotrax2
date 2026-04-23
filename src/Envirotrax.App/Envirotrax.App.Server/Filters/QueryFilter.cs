
using Microsoft.AspNetCore.Mvc.Filters;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Filters;

public class QueryFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments)
        {
            if (argument.Value != null && argument.Value.GetType() == typeof(Query))
            {
                var query = (Query)argument.Value;

                // when client is not passing DeletedById filter,
                // we add a new one to make database query return only active entries by default
                var isActiveFilter = query.Filter.Find(f => f.ColumnName == nameof(IDeleteAutitableModel<>.DeletedById));

                if (isActiveFilter == null)
                {
                    query.Filter.Add(new QueryProperty
                    {
                        ColumnName = nameof(IDeleteAutitableModel<>.DeletedById),
                        IsValueNull = true,
                        LogicalOperator = LogicalOperator.And
                    });
                }
            }

            base.OnActionExecuting(context);
        }
    }
}