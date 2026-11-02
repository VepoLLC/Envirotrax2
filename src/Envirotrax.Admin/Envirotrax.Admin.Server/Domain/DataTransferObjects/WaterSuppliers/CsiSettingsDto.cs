namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

public class CsiSettingsDto
{
    public int Id { get; set; }

    public int? ModificationGracePeriodDays { get; set; }
    public int NewlyCreatedBackflowTestExpirationDays { get; set; }
    public bool RequireInspectionImages { get; set; }

    public int ImpendingNotice1 { get; set; }
    public int ImpendingNotice2 { get; set; }
    public int PastDueNotice1 { get; set; }
    public int PastDueNotice2 { get; set; }
    public int NonCompliant1 { get; set; }
    public int NonCompliant2 { get; set; }
    public bool NonCompliantPrint1 { get; set; }
    public bool NonCompliantPrint2 { get; set; }

    public string ImpendingLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string ImpendingLettersForegroundColor { get; set; } = "#000000";
    public string ImpendingLettersBorderColor { get; set; } = "#000000";
    public string PastDueLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string PastDueLettersForegroundColor { get; set; } = "#000000";
    public string PastDueLettersBorderColor { get; set; } = "#000000";
    public string NonCompliantLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string NonCompliantLettersForegroundColor { get; set; } = "#000000";
    public string NonCompliantLettersBorderColor { get; set; } = "#000000";

    public string? NoticeBodyFont { get; set; }
    public int? NoticeBodyFontSize { get; set; }
    public int NoticeImageFitType { get; set; }
    public decimal NoticeImageFitSize { get; set; }
    public string? ImpendingTitle { get; set; }
    public string? ImpendingMessage { get; set; }
    public string? PastDueTitle { get; set; }
    public string? PastDueMessage { get; set; }
    public string? NonCompliantTitle { get; set; }
    public string? NonCompliantMessage { get; set; }
}
