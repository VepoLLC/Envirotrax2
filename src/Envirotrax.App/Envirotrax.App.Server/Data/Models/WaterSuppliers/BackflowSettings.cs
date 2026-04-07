using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.Common.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class BackflowSettings : TenantModel<WaterSupplier>
{
    public int TestingMethod { get; set; }
    public int? GracePeriodDays { get; set; }
    public bool AdjustBackflowCreepingDates { get; set; }
    public bool NewInstallationsRequireApproval { get; set; }
    public bool ReplacementsRequireApproval { get; set; }
    public bool DetectorAssembliesRequireMeterReading { get; set; }
    public bool OutOfServiceRequiresApproval { get; set; }
    public int OutOfServiceType { get; set; }
    public bool RequireBackflowTestImages { get; set; }

    public BackflowExpiringType ExpiringNotice1 { get; set; }
    public BackflowExpiringType ExpiringNotice2 { get; set; }
    public BackflowExpiredType ExpiredNotice1 { get; set; }
    public BackflowExpiredType ExpiredNotice2 { get; set; }
    public BackflowNonCompliantType BackflowNonCompliant1 { get; set; }
    public BackflowNonCompliantType BackflowNonCompliant2 { get; set; }

    public bool ShowWaterMeterNumber { get; set; }
    public bool ShowRainSensor { get; set; }
    public bool ShowOSSF { get; set; }
    public bool ShowPermitNumber { get; set; }

    [MaxLength(7)]
    public string ExpiringLettersBackgroundColor { get; set; } = "#d3d3d3";
    [MaxLength(7)]
    public string ExpiringLettersForegroundColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string ExpiringLettersBorderColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string ExpiredLettersBackgroundColor { get; set; } = "#d3d3d3";
    [MaxLength(7)]
    public string ExpiredLettersForegroundColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string ExpiredLettersBorderColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string NonCompliantLettersBackgroundColor { get; set; } = "#d3d3d3";
    [MaxLength(7)]
    public string NonCompliantLettersForegroundColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string NonCompliantLettersBorderColor { get; set; } = "#000000";
}
