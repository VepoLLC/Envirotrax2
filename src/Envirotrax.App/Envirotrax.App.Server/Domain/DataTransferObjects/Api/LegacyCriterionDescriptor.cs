namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

/// <summary>
/// Maps one V1 Where* parameter onto the V2 model, including the operator V1 emits. MinValue and
/// MaxValue reproduce the inclusive range checks V1 applies to some integer criteria.
/// </summary>
public class LegacyCriterionDescriptor
{
    public LegacyCriterionDescriptor(
        string parameterName,
        string legacyFieldName,
        LegacyFieldSource source,
        string propertyPath,
        LegacyCriterionOperator @operator,
        LegacyValueKind valueKind,
        int? minValue = null,
        int? maxValue = null)
    {
        ParameterName = parameterName;
        LegacyFieldName = legacyFieldName;
        Source = source;
        PropertyPath = propertyPath;
        Operator = @operator;
        ValueKind = valueKind;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public string ParameterName { get; }

    public string LegacyFieldName { get; }

    public LegacyFieldSource Source { get; }

    public string PropertyPath { get; }

    public LegacyCriterionOperator Operator { get; }

    public LegacyValueKind ValueKind { get; }

    public int? MinValue { get; }

    public int? MaxValue { get; }
}
