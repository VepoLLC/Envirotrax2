namespace Envirotrax.App.Server.Domain.DataTransferObjects.Api;

public class LegacySelectResult
{
    public LegacySelectResult(
        IReadOnlyList<string> columnNames,
        IReadOnlyList<object?[]> rows,
        IReadOnlyList<LegacyAppliedCriterion> appliedCriteria,
        string orderByField,
        string orderByDirection)
    {
        ColumnNames = columnNames;
        Rows = rows;
        AppliedCriteria = appliedCriteria;
        OrderByField = orderByField;
        OrderByDirection = orderByDirection;
    }

    public IReadOnlyList<string> ColumnNames { get; }

    public IReadOnlyList<object?[]> Rows { get; }

    public IReadOnlyList<LegacyAppliedCriterion> AppliedCriteria { get; }

    public string OrderByField { get; }

    public string OrderByDirection { get; }

    public int RecordCount => Rows.Count;
}
