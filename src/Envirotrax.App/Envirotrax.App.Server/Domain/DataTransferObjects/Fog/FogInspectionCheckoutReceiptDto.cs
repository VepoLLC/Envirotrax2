using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogInspectionCheckoutReceiptDto : ProfessionalCheckoutReceiptDto
{
    public List<FogInspectionDto> Inspections { get; set; } = [];
}
