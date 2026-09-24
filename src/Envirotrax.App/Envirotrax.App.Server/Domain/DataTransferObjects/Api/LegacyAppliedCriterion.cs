namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class LegacyAppliedCriterion
{
    public LegacyAppliedCriterion(string fieldName, string @operator, object value)
    {
        FieldName = fieldName;
        Operator = @operator;
        Value = value;
    }

    public string FieldName { get; }

    public string Operator { get; }

    public object Value { get; }
}
