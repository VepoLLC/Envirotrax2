
using Microsoft.AspNetCore.Mvc.Filters;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Data.Extensions;

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
                query.ExcludeDeleted();
            }

            base.OnActionExecuting(context);
        }
    }
}