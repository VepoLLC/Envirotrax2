
using System.Diagnostics;
using Envirotrax.Common.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Envirotrax.App.Server.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case AppValidationException appValidationException:
                var modelState = new ModelStateDictionary();
                modelState.AddModelError(string.Empty, appValidationException.Message);

                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(modelState));
                context.ExceptionHandled = true;

                break;

            case DuplicateRecordException duplicateRecordException:
                context.Result = new BadRequestObjectResult(duplicateRecordException.Message);
                context.ExceptionHandled = true;

                break;

            default:

                var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                context.Result = new ObjectResult(new
                {
                    message = "An unexpected error occurred. Please contact support and include this reference ID.",
                    traceId
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

                context.ExceptionHandled = true;

                break;
        }
    }
}
