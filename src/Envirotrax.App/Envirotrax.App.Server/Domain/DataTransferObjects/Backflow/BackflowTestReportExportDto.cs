namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestReportExportDto
{
    public BackflowTestReportDto Report { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
