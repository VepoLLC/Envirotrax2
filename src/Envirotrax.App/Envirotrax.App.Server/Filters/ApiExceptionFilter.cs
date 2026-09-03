
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
        }
    }
}
