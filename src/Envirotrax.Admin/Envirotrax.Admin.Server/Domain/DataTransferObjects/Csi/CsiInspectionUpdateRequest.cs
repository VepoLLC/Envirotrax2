
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionUpdateRequest
{
    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? MailingCompanyName { get; set; }

    public string? MailingContactName { get; set; }

    public string? MailingStreetNumber { get; set; }

    public string? MailingStreetName { get; set; }

    public string? MailingNumber { get; set; }

    public string? MailingCity { get; set; }

    public StateDto? MailingState { get; set; }

    public string? MailingZip { get; set; }

    public int ReasonForInspection { get; set; }

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

    public string? MaterialServiceLineOtherDescription { get; set; }

    public bool MaterialSolderLead { get; set; }
    public bool MaterialSolderLeadFree { get; set; }
    public bool MaterialSolderSolventWeld { get; set; }
    public bool MaterialSolderOther { get; set; }

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
