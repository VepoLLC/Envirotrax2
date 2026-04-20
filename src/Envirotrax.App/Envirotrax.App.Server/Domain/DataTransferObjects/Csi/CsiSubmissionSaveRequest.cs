using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiSubmissionSaveRequest
{
    [Required]
    public int SiteId { get; set; }

    [Required]
    public int WaterSupplierId { get; set; }

    public int? SelectedCsiAccountUserId { get; set; }

    public DateTime? InspectionDate { get; set; }

    public CsiInspectionReason ReasonForInspection { get; set; }

    public bool Compliance1 { get; set; }
    public bool Compliance2 { get; set; }
    public bool Compliance3 { get; set; }
    public bool Compliance4 { get; set; }
    public bool Compliance5 { get; set; }
    public bool Compliance6 { get; set; }

    public bool MaterialServiceLineLead { get; set; }
    public bool MaterialServiceLineCopper { get; set; }
    public bool MaterialServiceLinePVC { get; set; }
    public bool MaterialServiceLineOther { get; set; }
    public string? MaterialServiceLineOtherDescription { get; set; }

    public bool MaterialSolderLead { get; set; }
    public bool MaterialSolderLeadFree { get; set; }
    public bool MaterialSolderSolventWeld { get; set; }
    public bool MaterialSolderOther { get; set; }
    public string? MaterialSolderOtherDescription { get; set; }
    public string? Comments { get; set; }
}
