namespace Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

public class SiteAdditionalInformationDto
{
    public bool HasOnSiteSewageFacility { get; set; }

    public bool HasAuxWaterSupply { get; set; }

    public bool HasReclaimed { get; set; }

    public bool HasFireSystem { get; set; }

    public bool FireSeparateWater { get; set; }

    public bool HasGreaseTrap { get; set; }

    public bool HasGritTrap { get; set; }

    public bool HasIrrigation { get; set; }

    public bool IrrigationSeparateWater { get; set; }

    public bool HasDomesticPremisesIsolation { get; set; }

    public bool RequiresDomesticPremisesIsolation { get; set; }
}
