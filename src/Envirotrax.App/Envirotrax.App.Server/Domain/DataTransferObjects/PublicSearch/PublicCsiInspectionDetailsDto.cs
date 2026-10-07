
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;

public class PublicCsiInspectionDetailsDto : IDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public DateTime? InspectionDate { get; set; }

    public string? PropertyBusinessName { get; set; }
    public PropertyType PropertyType { get; set; }
    public string? PropertyStreetNumber { get; set; }
    public string? PropertyStreetName { get; set; }
    public string? PropertyNumber { get; set; }
    public string? PropertyCity { get; set; }
    public ReferencedStateDto? PropertyState { get; set; }
    public string? PropertyZip { get; set; }

    public string? MailingCompanyName { get; set; }
    public string? MailingContactName { get; set; }
    public string? MailingStreetNumber { get; set; }
    public string? MailingStreetName { get; set; }
    public string? MailingNumber { get; set; }
    public string? MailingCity { get; set; }
    public ReferencedStateDto? MailingState { get; set; }
    public string? MailingZip { get; set; }
    public string? MailingPhoneNumber { get; set; }
    public string? MailingEmailAddress { get; set; }

    public string? InspectorLicenseNumber { get; set; }
    public string? InspectorLicenseType { get; set; }
    public string? InspectorCompanyName { get; set; }
    public string? InspectorJobTitle { get; set; }
    public string? InspectorContactName { get; set; }
    public string? InspectorAddress { get; set; }
    public string? InspectorCity { get; set; }
    public string? InspectorState { get; set; }
    public string? InspectorZip { get; set; }
    public string? InspectorWorkNumber { get; set; }
    public string? InspectorCellNumber { get; set; }
    public string? InspectorFaxNumber { get; set; }

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

    public bool AiOssf { get; set; }
    public bool AiWaterWell { get; set; }
    public bool AiFireSystem { get; set; }
    public bool AiFireSystem2 { get; set; }
    public bool AiGreaseTrap { get; set; }
    public bool AiSandGrit { get; set; }
    public bool AiReclaimedWater { get; set; }
    public bool AiIrrigationSystem { get; set; }
    public bool AiIrrigationSystem2 { get; set; }
    public bool AiHasDomesticPremisesIsolation { get; set; }
    public bool AiRequiresDomesticPremisesIsolation { get; set; }

    public bool InspectionResult { get; set; }

    public string? Comments { get; set; }
}
