using Envirotrax.App.Server.Data.Models.Backflow;


namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class BackflowSettingsDto : IDto
{
    public int Id { get; set; }

    public BackflowTestingMethodType TestingMethod { get; set; }
    public int? GracePeriodDays { get; set; }
    public bool AdjustBackflowCreepingDates { get; set; }
    public bool NewInstallationsRequireApproval { get; set; }
    public bool ReplacementsRequireApproval { get; set; }
    public bool DetectorAssembliesRequireMeterReading { get; set; }
    public bool OutOfServiceRequiresApproval { get; set; }
    public BackflowOutOfServiceType OutOfServiceType { get; set; }
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

    public string ExpiringLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string ExpiringLettersForegroundColor { get; set; } = "#000000";
    public string ExpiringLettersBorderColor { get; set; } = "#000000";
    public string ExpiredLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string ExpiredLettersForegroundColor { get; set; } = "#000000";
    public string ExpiredLettersBorderColor { get; set; } = "#000000";
    public string NonCompliantLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string NonCompliantLettersForegroundColor { get; set; } = "#000000";
    public string NonCompliantLettersBorderColor { get; set; } = "#000000";
}
