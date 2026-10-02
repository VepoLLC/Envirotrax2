namespace Envirotrax.App.Server.Templates.Emails.Backflow;

public class BackflowTestCheckoutVm
{
    public string? PropertyBusinessName { get; set; }

    public string PropertyAddress { get; set; } = null!;

    public string? PropertyCityStateZip { get; set; }

    public List<BackflowTestCheckoutItemVm> Tests { get; set; } = [];
}

public class BackflowTestCheckoutItemVm
{
    public string? TestDate { get; set; }

    public string AssemblyDescription { get; set; } = null!;

    public string? SerialNumber { get; set; }

    public string? TestedBy { get; set; }
}
