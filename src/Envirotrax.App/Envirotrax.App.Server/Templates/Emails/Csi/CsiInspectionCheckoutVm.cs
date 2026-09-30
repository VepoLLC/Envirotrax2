namespace Envirotrax.App.Server.Templates.Emails.Csi;

public class CsiInspectionCheckoutVm
{
    public string? PropertyBusinessName { get; set; }

    public string PropertyAddress { get; set; } = null!;

    public string? PropertyCityStateZip { get; set; }

    public List<CsiInspectionCheckoutItemVm> Inspections { get; set; } = [];
}

public class CsiInspectionCheckoutItemVm
{
    public string? InspectionDate { get; set; }

    public string? InspectedBy { get; set; }
}
