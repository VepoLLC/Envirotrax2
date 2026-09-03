namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowComplianceSnapshotDto
{
    public DateTime ReportDate { get; set; }

    public int Total { get; set; }

    public int Compliant { get; set; }

    public int NonCompliant { get; set; }

    public double CompliantPercentage { get; set; }
}
