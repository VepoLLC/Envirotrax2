using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace Envirotrax.App.Server.Filters;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            message = "An unexpected error occurred. Please contact support and include this reference ID.",
            traceId
        }, cancellationToken);

        return true;
    }
}
