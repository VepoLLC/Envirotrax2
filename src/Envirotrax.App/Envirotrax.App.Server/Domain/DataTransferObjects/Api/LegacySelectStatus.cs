namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public enum LegacySelectStatus
{
    Success,
    SelectFieldsNotSpecified,
    UnsupportedField,
    CriteriaNotSpecified,
    CriterionValueInvalid,
    UnsupportedOrderBy,
    OrderByDirectionInvalid
}
