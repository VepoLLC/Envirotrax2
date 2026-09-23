namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowNewRemovedReportDto
{
    public List<BackflowNewRemovedPointDto> Points { get; set; } = [];
}

public class BackflowNewRemovedPointDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Label { get; set; } = "";
    public int Created { get; set; }
    public int Removed { get; set; }
}
