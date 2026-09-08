namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// V1's FOG inspection search offers the total capacity criteria as two fixed buckets
/// around 25% rather than as a free numeric range, so it is modelled as an enum instead
/// of a comparison filter on <see cref="Data.Models.Fog.FogInspection.TotalCapacityPercent"/>.
/// </summary>
public enum FogTotalCapacityRange
{
    TwentyFivePercentOrLess = 1,
    GreaterThanTwentyFivePercent = 2
}
