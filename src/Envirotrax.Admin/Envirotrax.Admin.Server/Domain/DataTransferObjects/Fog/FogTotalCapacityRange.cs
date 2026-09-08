namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;

/// <summary>
/// V1's FOG inspection search offers the total capacity criteria as two fixed buckets around 25%
/// rather than as a free numeric range.
/// </summary>
public enum FogTotalCapacityRange
{
    TwentyFivePercentOrLess = 1,
    GreaterThanTwentyFivePercent = 2
}
