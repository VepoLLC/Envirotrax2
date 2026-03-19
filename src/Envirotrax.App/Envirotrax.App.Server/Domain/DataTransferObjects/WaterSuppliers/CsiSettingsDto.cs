using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class CsiSettingsDto : IDto
{
    public int Id { get; set; }

    // CSI Settings
    public int? ModificationGracePeriodDays { get; set; }
    public int NewlyCreatedBackflowTestExpirationDays { get; set; }
    public bool RequireInspectionImages { get; set; }

    // CSI Letter Settings
    public CsiImpendingType ImpendingNotice1 { get; set; }
    public CsiImpendingType ImpendingNotice2 { get; set; }
    public CsiPastDueType PastDueNotice1 { get; set; }
    public CsiPastDueType PastDueNotice2 { get; set; }
    public CsiNonCompliantType NonCompliant1 { get; set; }
    public CsiNonCompliantType NonCompliant2 { get; set; }

    // CSI Letter Header Settings
    public string ImpendingLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string ImpendingLettersForegroundColor { get; set; } = "#000000";
    public string ImpendingLettersBorderColor { get; set; } = "#000000";
    public string PastDueLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string PastDueLettersForegroundColor { get; set; } = "#000000";
    public string PastDueLettersBorderColor { get; set; } = "#000000";
    public string NonCompliantLettersBackgroundColor { get; set; } = "#d3d3d3";
    public string NonCompliantLettersForegroundColor { get; set; } = "#000000";
    public string NonCompliantLettersBorderColor { get; set; } = "#000000";
}
