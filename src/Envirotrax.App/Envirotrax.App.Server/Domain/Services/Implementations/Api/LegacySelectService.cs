using System.Globalization;
using System.Linq.Expressions;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Api;
using Envirotrax.App.Server.Domain.DataTransferObjects.Api;
using Envirotrax.App.Server.Domain.Services.Definitions.Api;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Api;

/// <summary>
/// Builds and runs the legacy SELECT from the field and criterion descriptors. There is no
/// per-query code: the requested fields, criteria and ordering drive the expression tree.
/// </summary>
public class LegacySelectService : ILegacySelectService
{
    private static readonly CultureInfo LegacyCulture = CultureInfo.GetCultureInfo("en-US");

    private readonly ILegacyQueryRepository _legacyQueryRepository;
    private readonly ILegacyFieldMapService _fieldMap;

    public LegacySelectService(
        ILegacyQueryRepository legacyQueryRepository,
        ILegacyFieldMapService fieldMap)
    {
        _legacyQueryRepository = legacyQueryRepository;
        _fieldMap = fieldMap;
    }

    public async Task<LegacySelectOutcome> ExecuteAsync(LegacySelectRequest request, CancellationToken cancellationToken)
    {
        if (request.Table == LegacyApiTable.CsiBackflowSites)
        {
            return await ExecuteForTableAsync<Site>(request, siteNavigation: null, cancellationToken);
        }

        return await ExecuteForTableAsync<BackflowTest>(request, nameof(BackflowTest.Site), cancellationToken);
    }

    private async Task<LegacySelectOutcome> ExecuteForTableAsync<TEntity>(
        LegacySelectRequest request,
        string? siteNavigation,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        var fields = new List<LegacyFieldDescriptor>();

        if (request.SelectFields.Count == 0)
        {
            return LegacySelectOutcome.Failure(LegacySelectStatus.SelectFieldsNotSpecified, "[SelectFields] is required");
        }

        // Order and duplicates are preserved: each requested name becomes its own output column,
        // exactly as V1 emits one aliased column per entry.
        foreach (var wireName in request.SelectFields)
        {
            var descriptor = _fieldMap.FindField(request.Table, wireName);

            if (descriptor == null)
            {
                return LegacySelectOutcome.Failure(
                    LegacySelectStatus.UnsupportedField,
                    "The field [SelectFields].[" + wireName + "] is not an allowed field.");
            }

            fields.Add(descriptor);
        }

        var query = request.Table == LegacyApiTable.CsiBackflowSites
            ? (IQueryable<TEntity>)_legacyQueryRepository.QuerySites()
            : (IQueryable<TEntity>)_legacyQueryRepository.QueryBackflowTests();

        // V1 inner joins the site for every backflow test, so tests whose site does not resolve are
        // dropped from the result rather than returned with empty site columns.
        if (siteNavigation != null)
        {
            query = query.Where(BuildSitePresentPredicate<TEntity>(siteNavigation));
        }

        query = ApplySupplierScope(query, request.Scope);

        var appliedCriteria = new List<LegacyAppliedCriterion>();

        foreach (var criterion in request.Criteria)
        {
            var descriptor = _fieldMap.FindCriterion(request.Table, criterion.Key);

            // V1 silently ignores parameters it does not recognise.
            if (descriptor == null)
            {
                continue;
            }

            if (!TryParseValue(descriptor, criterion.Value, out var parsed, out var parseError))
            {
                return LegacySelectOutcome.Failure(LegacySelectStatus.CriterionValueInvalid, parseError!);
            }

            query = query.Where(BuildCriterionPredicate<TEntity>(descriptor, parsed!, siteNavigation));

            appliedCriteria.Add(new LegacyAppliedCriterion(
                descriptor.LegacyFieldName,
                RenderOperator(descriptor.Operator),
                parsed!));
        }

        if (appliedCriteria.Count == 0)
        {
            return LegacySelectOutcome.Failure(LegacySelectStatus.CriteriaNotSpecified, "SELECT criteria has not been specified.");
        }

        var orderOutcome = ApplyOrdering(ref query, request, siteNavigation, out var orderByField, out var orderByDirection);

        if (orderOutcome != null)
        {
            return orderOutcome;
        }

        var projected = query.Select(BuildProjection<TEntity>(fields, siteNavigation));
        var rows = await _legacyQueryRepository.ToListAsync(projected, cancellationToken);

        var columnNames = fields
            .Select(field => BuildColumnName(request.Table, field))
            .ToList();

        return LegacySelectOutcome.Success(new LegacySelectResult(columnNames, rows, appliedCriteria, orderByField, orderByDirection));
    }

    /// <summary>
    /// Reproduces V1's aliasing: bare names for Sites, Test./Site. prefixes for the joined tables.
    /// </summary>
    private static string BuildColumnName(LegacyApiTable table, LegacyFieldDescriptor field)
    {
        if (table == LegacyApiTable.CsiBackflowSites)
        {
            return field.WireName;
        }

        if (field.Source == LegacyFieldSource.Site)
        {
            return "Site." + field.WireName;
        }

        return "Test." + field.WireName;
    }

    private IQueryable<TEntity> ApplySupplierScope<TEntity>(IQueryable<TEntity> query, ApiSupplierScope scope)
        where TEntity : class
    {
        // An open account is unrestricted in V1, so no predicate is added.
        if (scope.Kind == ApiSupplierScopeKind.Open)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var member = Expression.PropertyOrField(parameter, "WaterSupplierId");

        Expression body;

        if (scope.Kind == ApiSupplierScopeKind.Master)
        {
            // V1 filters the data row's MasterWaterSupplierID; in V2 that hierarchy lives on
            // WaterSupplier.ParentId, already resolved into the child list by the scope service.
            var allowed = scope.AllowedWaterSupplierIds.ToList();
            var allowedExpression = CreateParameterExpression(allowed, typeof(List<int>));

            body = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Contains),
                new[] { typeof(int) },
                allowedExpression,
                member);
        }
        else
        {
            body = Expression.Equal(member, CreateParameterExpression(scope.WaterSupplierId, typeof(int)));
        }

        return query.Where(Expression.Lambda<Func<TEntity, bool>>(body, parameter));
    }

    private LegacySelectOutcome? ApplyOrdering<TEntity>(
        ref IQueryable<TEntity> query,
        LegacySelectRequest request,
        string? siteNavigation,
        out string orderByField,
        out string orderByDirection)
        where TEntity : class
    {
        orderByField = string.Empty;
        orderByDirection = "ASC";
        var descending = false;

        if (!string.IsNullOrEmpty(request.OrderByDirection))
        {
            if (string.Equals(request.OrderByDirection, "DESC", StringComparison.OrdinalIgnoreCase))
            {
                descending = true;
                orderByDirection = "DESC";
            }
            else if (!string.Equals(request.OrderByDirection, "ASC", StringComparison.OrdinalIgnoreCase))
            {
                return LegacySelectOutcome.Failure(
                    LegacySelectStatus.OrderByDirectionInvalid,
                    "Error validating [OrderByDirection]:  Invalid direction value.");
            }
        }

        // V1 defaults OrderBy to ID and always emits an ORDER BY clause.
        var orderByName = string.IsNullOrEmpty(request.OrderBy) ? "ID" : request.OrderBy;
        orderByField = orderByName;
        var descriptor = _fieldMap.FindOrderByField(request.Table, orderByName);

        if (descriptor == null)
        {
            return LegacySelectOutcome.Failure(
                LegacySelectStatus.UnsupportedOrderBy,
                "Error validating [OrderBy]:  The specified field is not an allowed field.");
        }

        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var member = BuildMemberAccess(parameter, descriptor.Source, descriptor.PropertyPath, siteNavigation);
        var selector = Expression.Lambda(member, parameter);

        var method = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

        var call = Expression.Call(
            typeof(Queryable),
            method,
            new[] { typeof(TEntity), member.Type },
            query.Expression,
            Expression.Quote(selector));

        query = query.Provider.CreateQuery<TEntity>(call);

        return null;
    }

    private static Expression<Func<TEntity, object[]>> BuildProjection<TEntity>(
        IReadOnlyList<LegacyFieldDescriptor> fields,
        string? siteNavigation)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");

        var items = fields
            .Select(field => (Expression)Expression.Convert(
                BuildMemberAccess(parameter, field.Source, field.PropertyPath, siteNavigation),
                typeof(object)))
            .ToList();

        var array = Expression.NewArrayInit(typeof(object), items);

        return Expression.Lambda<Func<TEntity, object[]>>(array, parameter);
    }

    private static Expression<Func<TEntity, bool>> BuildSitePresentPredicate<TEntity>(string siteNavigation)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var site = Expression.PropertyOrField(parameter, siteNavigation);
        var body = Expression.NotEqual(site, Expression.Constant(null, site.Type));

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    private static Expression<Func<TEntity, bool>> BuildCriterionPredicate<TEntity>(
        LegacyCriterionDescriptor descriptor,
        object value,
        string? siteNavigation)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var member = BuildMemberAccess(parameter, descriptor.Source, descriptor.PropertyPath, siteNavigation);
        var valueExpression = CreateParameterExpression(value, member.Type);

        Expression body;

        if (descriptor.Operator == LegacyCriterionOperator.GreaterThanOrEqual)
        {
            body = Expression.GreaterThanOrEqual(member, valueExpression);
        }
        else if (descriptor.Operator == LegacyCriterionOperator.LessThan)
        {
            body = Expression.LessThan(member, valueExpression);
        }
        else
        {
            body = Expression.Equal(member, valueExpression);
        }

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    /// <summary>
    /// The operator text V1 writes into the RequestSqlParameters echo.
    /// </summary>
    private static string RenderOperator(LegacyCriterionOperator @operator)
    {
        if (@operator == LegacyCriterionOperator.GreaterThanOrEqual)
        {
            return ">=";
        }

        if (@operator == LegacyCriterionOperator.LessThan)
        {
            return "<";
        }

        return "=";
    }

    private static Expression BuildMemberAccess(
        ParameterExpression parameter,
        LegacyFieldSource source,
        string propertyPath,
        string? siteNavigation)
    {
        Expression current = parameter;

        if (source == LegacyFieldSource.Site && siteNavigation != null)
        {
            current = Expression.PropertyOrField(current, siteNavigation);
        }

        foreach (var part in propertyPath.Split('.'))
        {
            current = Expression.PropertyOrField(current, part);
        }

        return current;
    }

    private static Expression CreateParameterExpression(object? value, Type targetType)
    {
        var converted = ConvertToTargetType(value, targetType);
        var holderType = typeof(ValueHolder<>).MakeGenericType(targetType);
        var holder = Activator.CreateInstance(holderType)!;

        holderType.GetProperty(nameof(ValueHolder<int>.Value))!.SetValue(holder, converted);

        return Expression.Property(Expression.Constant(holder, holderType), nameof(ValueHolder<int>.Value));
    }

    private static object? ConvertToTargetType(object? value, Type targetType)
    {
        if (value == null)
        {
            return null;
        }

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlying.IsInstanceOfType(value))
        {
            return value;
        }

        // V1 compares bit columns against an integer parameter.
        if (underlying == typeof(bool) && value is int boolInt)
        {
            return boolInt != 0;
        }

        if (underlying.IsEnum && value is int enumInt)
        {
            return Enum.ToObject(underlying, enumInt);
        }

        return Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
    }

    private static bool TryParseValue(
        LegacyCriterionDescriptor descriptor,
        string raw,
        out object? value,
        out string? error)
    {
        value = null;
        error = null;

        if (descriptor.ValueKind == LegacyValueKind.Int || descriptor.ValueKind == LegacyValueKind.Bool)
        {
            if (!int.TryParse(raw, NumberStyles.Integer, LegacyCulture, out var intValue))
            {
                error = "Invalid int value specified for field [" + descriptor.ParameterName + "]";
                return false;
            }

            if ((descriptor.MinValue != null && intValue < descriptor.MinValue)
                || (descriptor.MaxValue != null && intValue > descriptor.MaxValue))
            {
                error = "Invalid int value specified for field [" + descriptor.ParameterName + "]";
                return false;
            }

            value = intValue;
            return true;
        }

        if (descriptor.ValueKind == LegacyValueKind.DateTime || descriptor.ValueKind == LegacyValueKind.Date)
        {
            if (!DateTime.TryParse(raw, LegacyCulture, DateTimeStyles.None, out var dateValue))
            {
                error = "Invalid date value specified for field [" + descriptor.ParameterName + "]";
                return false;
            }

            value = dateValue;
            return true;
        }

        if (descriptor.ValueKind == LegacyValueKind.Number)
        {
            if (!double.TryParse(raw, NumberStyles.Float, LegacyCulture, out var doubleValue))
            {
                error = "Invalid number value specified for field [" + descriptor.ParameterName + "]";
                return false;
            }

            value = doubleValue;
            return true;
        }

        value = raw;
        return true;
    }

    private sealed class ValueHolder<T>
    {
        public T? Value { get; set; }
    }
}
