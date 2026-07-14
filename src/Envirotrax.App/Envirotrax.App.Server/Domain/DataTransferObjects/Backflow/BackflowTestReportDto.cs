namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestReportDto
{
    public int TotalCount { get; set; }
    public List<BackflowReportPeriodDto> Periods { get; set; } = [];
    public List<BackflowSubAccountReportItemDto> SubAccounts { get; set; } = [];
    public List<BackflowReportStatCategoryDto> Stats { get; set; } = [];
}

public class BackflowSubAccountReportItemDto
{
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class BackflowReportPeriodDto
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
}

public class BackflowReportStatCategoryDto
{
    public string Title { get; set; } = "";
    public List<BackflowReportStatItemDto> Items { get; set; } = [];
}

public class BackflowReportStatItemDto
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
}
