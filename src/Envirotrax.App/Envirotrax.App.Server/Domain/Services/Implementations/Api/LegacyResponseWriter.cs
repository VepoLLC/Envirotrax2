using System.Globalization;
using System.Xml.Linq;
using Envirotrax.App.Server.Domain.DataTransferObjects.Api;
using Envirotrax.App.Server.Domain.Services.Definitions.Api;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Api;

/// <summary>
/// Reproduces the V1 XML contract from EraResponse: a fixed four element header, then either
/// RequestSqlParameters + Recordset, or Errors. Every response is HTTP 200; the outcome is in the
/// body.
/// </summary>
public class LegacyResponseWriter : ILegacyResponseWriter
{
    public const string ContentType = "text/xml";

    private const string ApiVersion = "24.4.30.0";
    private const string ServerName = "IO API 1";
    private const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";

    // V1 formats every value with the server's culture. Production runs en-US, which is what the
    // documented samples show, so it is pinned here rather than inherited from the host.
    private static readonly CultureInfo LegacyCulture = CultureInfo.GetCultureInfo("en-US");

    public string WriteError(LegacyApiErrorCode code, string message)
    {
        var root = BuildHeader("ERROR");

        root.Add(new XElement("Errors",
            new XElement("Error",
                new XElement("ErrorNumber", ((int)code).ToString(CultureInfo.InvariantCulture)),
                new XElement("ErrorType", code.ToString()),
                new XElement("ErrorMessage", message))));

        return Render(root);
    }

    public string WriteRecordset(
        string tableNameAlias,
        ApiSupplierScope scope,
        IReadOnlyList<string> selectFields,
        LegacySelectResult result)
    {
        var root = BuildHeader("OK");

        root.Add(BuildRequestSqlParameters(tableNameAlias, scope, selectFields, result));
        root.Add(BuildRecordset(result));

        return Render(root);
    }

    private static XElement BuildHeader(string responseStatus)
    {
        return new XElement("EnvirotraxApiResponse",
            new XElement("ApiVersion", ApiVersion),
            new XElement("ServerName", ServerName),
            new XElement("ResponseUtcDate", DateTime.UtcNow.ToString(LegacyCulture)),
            new XElement("ResponseStatus", responseStatus));
    }

    private static XElement BuildRequestSqlParameters(
        string tableNameAlias,
        ApiSupplierScope scope,
        IReadOnlyList<string> selectFields,
        LegacySelectResult result)
    {
        var criteria = new XElement("SelectCriteria");

        // V1 lists the forced supplier restriction first, and omits it for an open account.
        if (scope.Kind == ApiSupplierScopeKind.Master)
        {
            criteria.Add(new XElement(
                "MasterWaterSupplierID",
                "MasterWaterSupplierID = " + scope.WaterSupplierId.ToString(CultureInfo.InvariantCulture)));
        }
        else if (scope.Kind == ApiSupplierScopeKind.Single)
        {
            criteria.Add(new XElement(
                "WaterSupplierID",
                "WaterSupplierID = " + scope.WaterSupplierId.ToString(CultureInfo.InvariantCulture)));
        }

        foreach (var criterion in result.AppliedCriteria)
        {
            criteria.Add(new XElement(
                criterion.FieldName,
                criterion.FieldName + " " + criterion.Operator + " " + FormatValue(criterion.Value)));
        }

        return new XElement("RequestSqlParameters",
            new XElement("TableName", tableNameAlias),
            new XElement("SelectFields", string.Join(", ", selectFields)),
            criteria,
            new XElement("SelectOrder",
                new XElement("OrderBy", result.OrderByField),
                new XElement("OrderByDirection", result.OrderByDirection)));
    }

    private static XElement BuildRecordset(LegacySelectResult result)
    {
        var recordset = new XElement("Recordset",
            new XElement("RecordCount", result.RecordCount.ToString(CultureInfo.InvariantCulture)));

        foreach (var row in result.Rows)
        {
            var record = new XElement("Record");

            // Columns are emitted in the requested order, including repeats: V1 writes one element
            // per reader column and does not de-duplicate.
            for (var index = 0; index < result.ColumnNames.Count; index++)
            {
                record.Add(new XElement(result.ColumnNames[index], FormatValue(row[index])));
            }

            recordset.Add(record);
        }

        return recordset;
    }

    /// <summary>
    /// Mirrors V1's dr.GetValue(fi).ToString(): NULL becomes an empty element, bool renders as
    /// True/False, and an enum renders as the underlying integer because V1 stored these as ints.
    /// </summary>
    private static string FormatValue(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is bool boolValue)
        {
            return boolValue ? "True" : "False";
        }

        var valueType = value.GetType();

        if (valueType.IsEnum)
        {
            return Convert.ToInt64(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
        }

        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, LegacyCulture);
        }

        return value.ToString() ?? string.Empty;
    }

    private static string Render(XElement root)
    {
        return XmlDeclaration + root.ToString();
    }
}
