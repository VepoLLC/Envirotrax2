using C = DocumentFormat.OpenXml.Drawing.Charts;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

internal static class BackflowChartAxisHelper
{
    public static C.CategoryAxis BuildCategoryAxis(uint axisId, uint crossAxisId)
    {
        return new C.CategoryAxis(
            new C.AxisId { Val = axisId },
            new C.Scaling(new C.Orientation { Val = C.OrientationValues.MinMax }),
            new C.Delete { Val = false },
            new C.AxisPosition { Val = C.AxisPositionValues.Bottom },
            new C.TickLabelPosition { Val = C.TickLabelPositionValues.Low },
            new C.CrossingAxis { Val = crossAxisId });
    }

    public static C.ValueAxis BuildValueAxis(uint axisId, uint crossAxisId, double? min = null, double? max = null)
    {
        var scaling = new C.Scaling(new C.Orientation { Val = C.OrientationValues.MinMax });

        if (max.HasValue)
        {
            scaling.MaxAxisValue = new C.MaxAxisValue { Val = max.Value };
        }

        if (min.HasValue)
        {
            scaling.MinAxisValue = new C.MinAxisValue { Val = min.Value };
        }

        return new C.ValueAxis(
            new C.AxisId { Val = axisId },
            scaling,
            new C.Delete { Val = false },
            new C.AxisPosition { Val = C.AxisPositionValues.Left },
            new C.MajorGridlines(),
            new C.CrossingAxis { Val = crossAxisId });
    }

    public static C.StringLiteral BuildStringLiteral(IReadOnlyList<string> values)
    {
        var literal = new C.StringLiteral(new C.PointCount { Val = (uint)values.Count });

        for (var i = 0; i < values.Count; i++)
        {
            literal.Append(new C.StringPoint { Index = (uint)i, NumericValue = new C.NumericValue(values[i]) });
        }

        return literal;
    }

    public static C.NumberLiteral BuildNumberLiteral(IReadOnlyList<double> values)
    {
        var literal = new C.NumberLiteral(new C.FormatCode("General"), new C.PointCount { Val = (uint)values.Count });

        for (var i = 0; i < values.Count; i++)
        {
            literal.Append(new C.NumericPoint { Index = (uint)i, NumericValue = new C.NumericValue(values[i].ToString(System.Globalization.CultureInfo.InvariantCulture)) });
        }

        return literal;
    }
}
