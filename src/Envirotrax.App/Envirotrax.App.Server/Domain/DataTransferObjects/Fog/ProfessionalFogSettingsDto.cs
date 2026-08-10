
namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class ProfessionalFogSettingsDto
{
    public bool FogTransportersRequireInsurance { get; set; }
    public decimal? FogTransportersRequireInsuranceAmount { get; set; }
    public bool FogVehiclesRequirePermit { get; set; }
    public bool FogVehiclesRequireInspection { get; set; }
}
