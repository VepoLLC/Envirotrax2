namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionApprovalRequest
{
    public bool Disapproved { get; set; }
    public string? DisapprovedReason { get; set; }
}
