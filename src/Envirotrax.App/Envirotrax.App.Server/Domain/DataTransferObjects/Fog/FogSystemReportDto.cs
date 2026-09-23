namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public enum FogTripTicketReportDateType
{
    GeneratorRemovalDate = 0,
    ReceiverDeliveryDate = 1
}

public class FogSystemReportDto
{
    public int TotalCount { get; set; }
    public double TotalGallons { get; set; }
    public double TotalCubicFeet { get; set; }
    public List<FogReportPeriodDto> Periods { get; set; } = [];
    public List<FogSubAccountReportItemDto> SubAccounts { get; set; } = [];
    public List<FogReportStatCategoryDto> Stats { get; set; } = [];
}

public class FogReportPeriodDto
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double Gallons { get; set; }
    public double CubicFeet { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
}

public class FogSubAccountReportItemDto
{
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double Gallons { get; set; }
    public double CubicFeet { get; set; }
}

public class FogReportStatCategoryDto
{
    public string Title { get; set; } = "";
    public List<FogReportStatItemDto> Items { get; set; } = [];
}

public class FogReportStatItemDto
{
    public string Label { get; set; } = "";
    public int Count { get; set; }
    public double Percentage { get; set; }
    public double Gallons { get; set; }
    public double CubicFeet { get; set; }
}
