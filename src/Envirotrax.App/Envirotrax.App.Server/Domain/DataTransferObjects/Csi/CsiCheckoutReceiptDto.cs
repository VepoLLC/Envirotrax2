using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiCheckoutReceiptDto : ProfessionalCheckoutReceiptDto
{
    public List<CsiInspectionDto> Inspections { get; set; } = [];
}
