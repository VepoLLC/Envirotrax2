namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class LegacySelectRequest
{
    public LegacySelectRequest(
        LegacyApiTable table,
        ApiSupplierScope scope,
        IReadOnlyList<string> selectFields,
        IReadOnlyList<KeyValuePair<string, string>> criteria,
        string? orderBy,
        string? orderByDirection)
    {
        Table = table;
        Scope = scope;
        SelectFields = selectFields;
        Criteria = criteria;
        OrderBy = orderBy;
        OrderByDirection = orderByDirection;
    }

    public LegacyApiTable Table { get; }

    public ApiSupplierScope Scope { get; }

    public IReadOnlyList<string> SelectFields { get; }

    public IReadOnlyList<KeyValuePair<string, string>> Criteria { get; }

    public string? OrderBy { get; }

    public string? OrderByDirection { get; }
}
