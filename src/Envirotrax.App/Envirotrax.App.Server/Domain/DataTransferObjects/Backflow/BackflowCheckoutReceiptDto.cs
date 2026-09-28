using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowCheckoutReceiptDto : ProfessionalCheckoutReceiptDto
{
    public List<BackflowTestDto> Tests { get; set; } = [];
}
