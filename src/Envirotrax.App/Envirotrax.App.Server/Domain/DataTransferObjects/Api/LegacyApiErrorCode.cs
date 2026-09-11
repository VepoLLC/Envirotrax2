namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public enum LegacyApiErrorCode
{
    Unknown = 100,
    NoParameters = 102,
    CredentialsUnspecified = 200,
    AuthorizationFailed = 201,
    FormatTypeInvalid = 209,
    RequestTypeNotSpecified = 210,
    RequestTypeInvalid = 211,
    TableNameNotSpecified = 220,
    TableNameInvalid = 221,
    TablePermission = 222,
    RequestSelectFieldsNotSpecified = 300,
    RequestSelectFieldInvalid = 301,
    RequestSelectCriteriaFieldInvalid = 311,
    RequestSelectOrderByError = 313,
    RequestSelectCriteriaPostValidation = 315
}
