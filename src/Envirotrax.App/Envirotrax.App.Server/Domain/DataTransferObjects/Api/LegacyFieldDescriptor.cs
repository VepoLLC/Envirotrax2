using System.Linq.Expressions;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

/// <summary>
/// Maps one V1 wire field name onto the V2 model. PropertyPath is relative to the entity named by
/// Source and may traverse navigations (for example "State.Code").
/// </summary>
public class LegacyFieldDescriptor
{
    public LegacyFieldDescriptor(
        string wireName,
        LegacyFieldSource source,
        string propertyPath,
        LegacyValueKind valueKind,
        Func<Expression, Expression>? valueTransform = null)
    {
        WireName = wireName;
        Source = source;
        PropertyPath = propertyPath;
        ValueKind = valueKind;
        ValueTransform = valueTransform;
    }

    public string WireName { get; }

    public LegacyFieldSource Source { get; }

    public string PropertyPath { get; }

    public LegacyValueKind ValueKind { get; }

    /// <summary>
    /// Optional transform applied to the resolved PropertyPath expression to produce the wire
    /// value, for V1 fields V2 stores differently rather than as a column of their own. It must
    /// stay translatable to SQL: it is composed into the same projection, never evaluated locally.
    /// </summary>
    public Func<Expression, Expression>? ValueTransform { get; }
}
