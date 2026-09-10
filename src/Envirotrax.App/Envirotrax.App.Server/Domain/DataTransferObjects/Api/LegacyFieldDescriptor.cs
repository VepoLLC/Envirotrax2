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
        LegacyValueKind valueKind)
    {
        WireName = wireName;
        Source = source;
        PropertyPath = propertyPath;
        ValueKind = valueKind;
    }

    public string WireName { get; }

    public LegacyFieldSource Source { get; }

    public string PropertyPath { get; }

    public LegacyValueKind ValueKind { get; }
}
