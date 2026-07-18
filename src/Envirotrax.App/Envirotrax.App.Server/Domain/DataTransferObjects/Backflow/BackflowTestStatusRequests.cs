namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestScheduleMonthRequest
{
    public int Month { get; set; }
}

public class BackflowTestDisapprovalRequest
{
    public bool Disapproved { get; set; }
}

public class BackflowTestRejectionRequest
{
    public bool Rejected { get; set; }
    public string? RejectedReason { get; set; }
}

public class BackflowTestForceRenewalRequest
{
    public bool ForceRenewal { get; set; }
    public int? ForceRenewalYears { get; set; }
}
