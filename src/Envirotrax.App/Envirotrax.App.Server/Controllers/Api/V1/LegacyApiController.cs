using Envirotrax.App.Server.Data.Models.Api;
using Envirotrax.App.Server.Domain.DataTransferObjects.Api;
using Envirotrax.App.Server.Domain.Services.Definitions.Api;
using Envirotrax.App.Server.Domain.Services.Implementations.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Api.V1;

/// <summary>
/// V1-compatible API endpoint. Deliberately does not derive from EnvirotraxBaseController: that
/// base carries an inherited HasScope attribute, which would reject callers holding only a legacy
/// UserID/ApiKey credential.
/// </summary>
[AllowAnonymous]
[ApiController]
public class LegacyApiController : ControllerBase
{
    private readonly IApiAuthenticationService _apiAuthenticationService;
    private readonly IApiSupplierScopeService _apiSupplierScopeService;
    private readonly ILegacySelectService _legacySelectService;
    private readonly ILegacyResponseWriter _legacyResponseWriter;
    private readonly ILogger<LegacyApiController> _logger;

    public LegacyApiController(
        IApiAuthenticationService apiAuthenticationService,
        IApiSupplierScopeService apiSupplierScopeService,
        ILegacySelectService legacySelectService,
        ILegacyResponseWriter legacyResponseWriter,
        ILogger<LegacyApiController> logger)
    {
        _apiAuthenticationService = apiAuthenticationService;
        _apiSupplierScopeService = apiSupplierScopeService;
        _legacySelectService = legacySelectService;
        _legacyResponseWriter = legacyResponseWriter;
        _logger = logger;
    }

    /// <summary>
    /// Maps an execution outcome onto the closest V1 error code.
    /// </summary>
    private static LegacyApiErrorCode MapSelectError(LegacySelectStatus status)
    {
        if (status == LegacySelectStatus.SelectFieldsNotSpecified)
        {
            return LegacyApiErrorCode.RequestSelectFieldsNotSpecified;
        }

        if (status == LegacySelectStatus.UnsupportedField)
        {
            return LegacyApiErrorCode.RequestSelectFieldInvalid;
        }

        if (status == LegacySelectStatus.CriteriaNotSpecified)
        {
            return LegacyApiErrorCode.RequestSelectCriteriaPostValidation;
        }

        if (status == LegacySelectStatus.UnsupportedOrderBy || status == LegacySelectStatus.OrderByDirectionInvalid)
        {
            return LegacyApiErrorCode.RequestSelectOrderByError;
        }

        return LegacyApiErrorCode.RequestSelectCriteriaFieldInvalid;
    }

    [HttpPost("/")]
    public async Task<IActionResult> ProcessAsync(CancellationToken cancellationToken)
    {
        // V1 wraps the whole pipeline in a catch that reports the failure as error 100, so the
        // caller still receives HTTP 200 with an XML body instead of a framework error page.
        try
        {
            return await ProcessCoreAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Legacy API request failed.");

            return LegacyError(LegacyApiErrorCode.Unknown, "API Error:" + exception.Message);
        }
    }

    private async Task<IActionResult> ProcessCoreAsync(CancellationToken cancellationToken)
    {
        // V1 keys off an empty form collection rather than the content type, so a body it cannot
        // parse into form fields is error 102 rather than an unsupported-media-type rejection.
        if (!Request.HasFormContentType || Request.Form.Count == 0)
        {
            return LegacyError(LegacyApiErrorCode.NoParameters, "No request parameters sent");
        }

        var request = LegacyApiRequest.Parse(Request.Form);

        // V1 validates FormatType before authentication, so an invalid value is reported even when
        // the credentials are wrong. A valid value is accepted but not yet honoured: JSON rendering
        // is still outstanding.
        if (!IsFormatTypeValid(request.FormatType))
        {
            return LegacyError(
                LegacyApiErrorCode.FormatTypeInvalid,
                "Invalid [FormatType] value specified.  Value of \"XML\" or \"JSON\" expected.");
        }

        if (!request.HasCredentials())
        {
            return LegacyError(LegacyApiErrorCode.CredentialsUnspecified, "One more credential fields have not been provided");
        }

        var account = await _apiAuthenticationService.AuthenticateAsync(request.UserId, request.ApiKey, cancellationToken);

        if (account == null)
        {
            return LegacyError(LegacyApiErrorCode.AuthorizationFailed, "Authorization Failed:  Invalid API credentials.");
        }

        // V1 resolves the supplier inside authentication, so a missing supplier row is reported as
        // an authorization failure and takes precedence over any request validation below.
        var scope = await _apiSupplierScopeService.ResolveAsync(account, cancellationToken);

        if (scope == null)
        {
            return LegacyError(
                LegacyApiErrorCode.AuthorizationFailed,
                "Authorization Failed:  The water supplier record specified for this API account does not exist.  Please contact the administrator for assistance.");
        }

        var requestTypeError = ResolveRequestType(request.RequestType, out var requestType);

        if (requestTypeError != null)
        {
            return requestTypeError;
        }

        var tableError = ResolveTable(request.TableName, requestType, account, out var table);

        if (tableError != null)
        {
            return tableError;
        }

        if (requestType == LegacyApiRequestType.Update)
        {
            return LegacyError(LegacyApiErrorCode.Unknown, "API Error:UPDATE is not supported by this API.");
        }

        var selectRequest = new LegacySelectRequest(
            table,
            scope,
            request.ParseSelectFields(),
            request.OtherParameters,
            request.OrderBy,
            request.OrderByDirection);

        var outcome = await _legacySelectService.ExecuteAsync(selectRequest, cancellationToken);

        if (outcome.Status != LegacySelectStatus.Success)
        {
            return LegacyError(MapSelectError(outcome.Status), outcome.Message!);
        }

        var xml = _legacyResponseWriter.WriteRecordset(
            request.TableName!,
            scope,
            selectRequest.SelectFields,
            outcome.Result!);

        return Content(xml, LegacyResponseWriter.ContentType);
    }

    /// <summary>
    /// V1 only rejects a FormatType that was supplied and is neither XML nor JSON; an absent value
    /// defaults to XML.
    /// </summary>
    private static bool IsFormatTypeValid(string? value)
    {
        if (value == null)
        {
            return true;
        }

        return string.Equals(value, "XML", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "JSON", StringComparison.OrdinalIgnoreCase);
    }

    private ContentResult? ResolveRequestType(string? value, out LegacyApiRequestType requestType)
    {
        requestType = LegacyApiRequestType.Select;

        if (value == null)
        {
            return LegacyError(LegacyApiErrorCode.RequestTypeNotSpecified, "[RequestType] is required");
        }

        if (string.Equals(value, "SELECT", StringComparison.OrdinalIgnoreCase))
        {
            requestType = LegacyApiRequestType.Select;
            return null;
        }

        if (string.Equals(value, "UPDATE", StringComparison.OrdinalIgnoreCase))
        {
            requestType = LegacyApiRequestType.Update;
            return null;
        }

        return LegacyError(LegacyApiErrorCode.RequestTypeInvalid, "Invalid [RequestType] value specified");
    }

    private ContentResult? ResolveTable(
        string? alias,
        LegacyApiRequestType requestType,
        ApiAccount account,
        out LegacyApiTable table)
    {
        table = LegacyApiTable.CsiBackflowSites;

        if (alias == null || alias == "")
        {
            return LegacyError(LegacyApiErrorCode.TableNameNotSpecified, "[TableName] is required");
        }

        ApiPermissionLevel permission;

        if (string.Equals(alias, "SITES", StringComparison.OrdinalIgnoreCase))
        {
            table = LegacyApiTable.CsiBackflowSites;
            permission = account.PermissionSites;
        }
        else if (string.Equals(alias, "BACKFLOWTESTS", StringComparison.OrdinalIgnoreCase))
        {
            table = LegacyApiTable.SaveBackflowDeviceTests;
            permission = account.PermissionBackflowTests;
        }
        else
        {
            return LegacyError(LegacyApiErrorCode.TableNameInvalid, "Invalid [TableName] value specified");
        }

        if (requestType == LegacyApiRequestType.Select)
        {
            if (permission == ApiPermissionLevel.Deny)
            {
                return LegacyError(
                    LegacyApiErrorCode.TablePermission,
                    "Table Permission Error: [SELECT] [RequestType] for table [" + alias + "] is denied");
            }

            return null;
        }

        if (permission != ApiPermissionLevel.ReadWrite)
        {
            return LegacyError(
                LegacyApiErrorCode.TablePermission,
                "Table Permission Error: [UPDATE] [RequestType] for table [" + alias + "] is denied");
        }

        return null;
    }

    private ContentResult LegacyError(LegacyApiErrorCode code, string message)
    {
        // V1 answers every request with HTTP 200 and reports the outcome in the body.
        return Content(_legacyResponseWriter.WriteError(code, message), LegacyResponseWriter.ContentType);
    }
}
