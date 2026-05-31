namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiSystemReportDto
{
    public int TotalCount { get; set; }
    public List<CsiReportPeriodDto> Periods { get; set; } = [];
    public List<CsiSubAccountReportItemDto> SubAccounts { get; set; } = [];
    public List<CsiReportStatCategoryDto> Stats { get; set; } = [];
}

public class CsiSubAccountReportItemDto
{
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CsiReportPeriodDto
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
}

public class CsiReportStatCategoryDto
{
    public string Title { get; set; } = "";
    public List<CsiReportStatItemDto> Items { get; set; } = [];
}

public class CsiReportStatItemDto
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
}
