namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowComplianceReportExportDto
{
    public BackflowComplianceReportDto Report { get; set; } = new();
    public bool IgnoreLast30Days { get; set; }
}
