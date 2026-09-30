namespace Envirotrax.App.Server.Templates.Emails.Fog;

public class FogTripTicketCheckoutVm
{
    public string? PropertyBusinessName { get; set; }

    public string PropertyAddress { get; set; } = null!;

    public string? PropertyCityStateZip { get; set; }

    public List<FogTripTicketCheckoutItemVm> Tickets { get; set; } = [];
}

public class FogTripTicketCheckoutItemVm
{
    public string? TransporterName { get; set; }

    public string? Vehicle { get; set; }

    public string? WasteRemoved { get; set; }

    public string? WasteRemovedDate { get; set; }

    public string? Receiver { get; set; }
}
