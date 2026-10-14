using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

/// <summary>
/// Makes an "Account Number" criterion also match the water supplier's own WS account number when the site's
/// supplier has GeneralSettings.IncludeWsAccountNumbers turned on, as every V1 account number search did.
/// </summary>
/// <remarks>
/// The filter library folds a group's children left to right with no precedence, and it can't nest groups (a
/// grandchild group recurses into its siblings). So the criterion becomes one flat chain,
/// [WS account number, supplier setting (And), account number (Or)], which reads as
/// (WS account number AND supplier setting) OR account number. The search bar already sends its criteria as one
/// OR'ed group, so there the chain goes at the front of that group instead.
/// </remarks>
public static class WaterSupplierAccountNumberSearch
{
    private const string IncludeWsAccountNumbersPath = "WaterSupplier.GeneralSettings.IncludeWsAccountNumbers";

    private static readonly ComparisonOperator[] MatchingOperators =
    [
        ComparisonOperator.Eq,
        ComparisonOperator.Ct,
        ComparisonOperator.StW,
        ComparisonOperator.EndW
    ];

    /// <param name="accountNumberColumn">
    /// The model path the account number criterion arrives on: AccountNumber on a site or a backflow test,
    /// Site.AccountNumber on an inspection, trip ticket or site log.
    /// </param>
    /// <param name="siteNavigation">The navigation from the searched record to its site; null when it is the site.</param>
    public static void Apply(Query query, string accountNumberColumn, string? siteNavigation = null)
    {
        var sitePrefix = siteNavigation == null ? string.Empty : $"{siteNavigation}.";

        for (var index = 0; index < query.Filter.Count; index++)
        {
            var criterion = query.Filter[index];

            if (IsAccountNumberCriterion(criterion, accountNumberColumn))
            {
                // The group's own column has no value, so it matches every row and only its children decide.
                query.Filter[index] = new QueryProperty
                {
                    ColumnName = accountNumberColumn,
                    LogicalOperator = criterion.LogicalOperator,
                    Children = CreateChain([criterion], [], accountNumberColumn, sitePrefix)
                };
            }
            else if (!criterion.Children.IsNullOrEmpty())
            {
                ApplyToSearchBarGroup(criterion, accountNumberColumn, sitePrefix);
            }
        }
    }

    /// <summary>For records searched by their site's account number (Site.AccountNumber).</summary>
    public static void ApplyThroughSite(Query query, string siteNavigation)
    {
        Apply(query, $"{siteNavigation}.{nameof(Site.AccountNumber)}", siteNavigation);
    }

    private static void ApplyToSearchBarGroup(QueryProperty group, string accountNumberColumn, string sitePrefix)
    {
        var accountNumberCriteria = group.Children
            .Where(child => IsAccountNumberCriterion(child, accountNumberColumn))
            .ToList();

        // Reordering only keeps the group's meaning when every child is OR'ed. The first child's operator is
        // never read, so it doesn't count.
        var isOrGroup = group.Children
            .Skip(1)
            .All(child => child.LogicalOperator == LogicalOperator.Or);

        if (accountNumberCriteria.Count == 0 || !isOrGroup)
        {
            return;
        }

        var otherCriteria = group.Children
            .Except(accountNumberCriteria)
            .ToList();

        foreach (var otherCriterion in otherCriteria)
        {
            otherCriterion.LogicalOperator = LogicalOperator.Or;
        }

        group.Children = CreateChain(accountNumberCriteria, otherCriteria, accountNumberColumn, sitePrefix);
    }

    // Reads as (WS account number OR ... AND supplier setting) OR account number OR ... OR the other criteria.
    private static QueryFilter CreateChain(
        List<QueryProperty> accountNumberCriteria,
        List<QueryProperty> otherCriteria,
        string accountNumberColumn,
        string sitePrefix)
    {
        var chain = new QueryFilter();

        chain.AddRange(accountNumberCriteria.Select(criterion =>
            CreateCriterion(sitePrefix + nameof(Site.WaterSupplierAccountNumber), criterion)));

        chain.Add(new QueryProperty
        {
            ColumnName = sitePrefix + IncludeWsAccountNumbersPath,
            Value = bool.TrueString,
            LogicalOperator = LogicalOperator.And
        });

        chain.AddRange(accountNumberCriteria.Select(criterion => CreateCriterion(accountNumberColumn, criterion)));
        chain.AddRange(otherCriteria);

        return chain;
    }

    private static bool IsAccountNumberCriterion(QueryProperty criterion, string accountNumberColumn)
    {
        return string.Equals(criterion.ColumnName, accountNumberColumn, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(criterion.Value)
            && !criterion.IsValueNull
            && criterion.Children.IsNullOrEmpty()
            && MatchingOperators.Contains(criterion.ComparisonOperator);
    }

    private static QueryProperty CreateCriterion(string columnName, QueryProperty criterion)
    {
        return new QueryProperty
        {
            ColumnName = columnName,
            Value = criterion.Value,
            ComparisonOperator = criterion.ComparisonOperator,
            LogicalOperator = LogicalOperator.Or
        };
    }
}
