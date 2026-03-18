using Envirotrax.Common.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class CsiSettings : TenantModel<WaterSupplier>
{
    // CSI Settings
    public int? ModificationGracePeriodDays { get; set; }
    public int NewlyCreatedBackflowTestExpirationDays { get; set; }
    public bool RequireInspectionImages { get; set; }

    public int ImpendingNotice1 { get; set; }
    public int ImpendingNotice2 { get; set; }
    public int PastDueNotice1 { get; set; }
    public int PastDueNotice2 { get; set; }
    public int NonCompliant1 { get; set; }
    public int NonCompliant2 { get; set; }


    [MaxLength(7)]
    public string ImpendingLettersBackgroundColor { get; set; } = "#d3d3d3";
    [MaxLength(7)]
    public string ImpendingLettersForegroundColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string ImpendingLettersBorderColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string PastDueLettersBackgroundColor { get; set; } = "#d3d3d3";
    [MaxLength(7)]
    public string PastDueLettersForegroundColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string PastDueLettersBorderColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string NonCompliantLettersBackgroundColor { get; set; } = "#d3d3d3";
    [MaxLength(7)]
    public string NonCompliantLettersForegroundColor { get; set; } = "#000000";
    [MaxLength(7)]
    public string NonCompliantLettersBorderColor { get; set; } = "#000000";
}
