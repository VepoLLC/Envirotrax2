namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowComplianceReportDto
{
    public int TotalActive { get; set; }
    public int Compliant { get; set; }
    public int NonCompliant { get; set; }
    public double CompliantPercentage { get; set; }
    public double NonCompliantPercentage { get; set; }
    public List<BackflowComplianceRequirementDto> Requirements { get; set; } = [];
}

public class BackflowComplianceRequirementDto
{
    public string PropertyType { get; set; } = "";
    public string AssemblyType { get; set; } = "";
    public string HazardType { get; set; } = "";
    public bool HasSiteOssf { get; set; }
    public bool AuxWaterSupply { get; set; }
    public int RenewalYears { get; set; }
    public int Active { get; set; }
    public int Compliant { get; set; }
    public double Percentage { get; set; }
}
