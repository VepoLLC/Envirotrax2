using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionAdminUpdateRequest
{
    public PropertyType PropertyType { get; set; }

    [StringLength(100)]
    public string? PropertyBusinessName { get; set; }

    [StringLength(50)]
    public string? PropertyStreetNumber { get; set; }

    [StringLength(100)]
    public string? PropertyStreetName { get; set; }

    [StringLength(50)]
    public string? PropertyNumber { get; set; }

    [StringLength(50)]
    public string? PropertyCity { get; set; }

    public ReferencedStateDto? PropertyState { get; set; }

    [StringLength(20)]
    public string? PropertyZip { get; set; }

    [StringLength(100)]
    public string? MailingCompanyName { get; set; }

    [StringLength(100)]
    public string? MailingContactName { get; set; }

    [StringLength(50)]
    public string? MailingStreetNumber { get; set; }

    [StringLength(100)]
    public string? MailingStreetName { get; set; }

    [StringLength(50)]
    public string? MailingNumber { get; set; }

    [StringLength(50)]
    public string? MailingCity { get; set; }

    public ReferencedStateDto? MailingState { get; set; }

    [StringLength(20)]
    public string? MailingZip { get; set; }

    public CsiInspectionReason ReasonForInspection { get; set; }

    public DateTime? InspectionDate { get; set; }

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

    [StringLength(200)]
    public string? MaterialServiceLineOtherDescription { get; set; }

    public bool MaterialSolderLead { get; set; }
    public bool MaterialSolderLeadFree { get; set; }
    public bool MaterialSolderSolventWeld { get; set; }
    public bool MaterialSolderOther { get; set; }

    [StringLength(200)]
    public string? MaterialSolderOtherDescription { get; set; }

    public bool AiOssf { get; set; }
    public bool AiWaterWell { get; set; }
    public bool AiFireSystem { get; set; }
    public bool AiFireSystem2 { get; set; }
    public bool AiGreaseTrap { get; set; }
    public bool AiSandGrit { get; set; }
    public bool AiReclaimedWater { get; set; }
    public bool AiIrrigationSystem { get; set; }
    public bool AiIrrigationSystem2 { get; set; }

    public string? Comments { get; set; }
}
