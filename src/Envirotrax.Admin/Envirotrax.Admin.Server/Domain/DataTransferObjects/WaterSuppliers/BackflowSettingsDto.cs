
namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

public class BackflowSettingsDto
{
    public int Id { get; set; }

    public int TestingMethod { get; set; }
    public int? GracePeriodDays { get; set; }
    public bool AdjustBackflowCreepingDates { get; set; }
    public bool NewInstallationsRequireApproval { get; set; }
    public bool ReplacementsRequireApproval { get; set; }
    public bool DetectorAssembliesRequireMeterReading { get; set; }
    public bool OutOfServiceRequiresApproval { get; set; }
    public int OutOfServiceType { get; set; }
    public bool RequireBackflowTestImages { get; set; }

    public int ExpiringNotice1 { get; set; }
    public int ExpiringNotice2 { get; set; }
    public int ExpiredNotice1 { get; set; }
    public int ExpiredNotice2 { get; set; }
    public int BackflowNonCompliant1 { get; set; }
    public int BackflowNonCompliant2 { get; set; }

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

    public string? NoticeBodyFont { get; set; }
    public int? NoticeBodyFontSize { get; set; }
    public string? ExpiringTitle { get; set; }
    public string? ExpiringMessage { get; set; }
    public string? ExpiredTitle { get; set; }
    public string? ExpiredMessage { get; set; }
    public string? NonCompliantTitle { get; set; }
    public string? NonCompliantMessage { get; set; }
}


