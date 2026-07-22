namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowComplianceHistoryDto
{
    public List<BackflowComplianceHistoryPointDto> Points { get; set; } = [];
}

public class BackflowComplianceHistoryPointDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; } = "";
    public int Total { get; set; }
    public int Compliant { get; set; }
    public int NonCompliant { get; set; }
    public double Percentage { get; set; }
}
