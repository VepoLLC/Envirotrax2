using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class BackflowRenewalRequirementDto : IDto
{
    public int Id { get; set; }

    public PropertyType PropertyType { get; set; }

    [MaxLength(50)]
    public string? DeviceType { get; set; }

    [MaxLength(50)]
    public string? HazardType { get; set; }

    public bool HasSiteOssf { get; set; }

    public bool AuxWaterSupply { get; set; }

    public int RenewalYears { get; set; }
}
