
using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalWaterSupplierDto
{
    [Required]
    public ReferencedWaterSupplierDto WaterSupplier { get; set; } = null!;

    public ReferencedProfessionalDto? Professional { get; set; }

    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInspection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }

    public bool IsBanned { get; set; }

    public decimal? BackflowResidentialTestFee { get; set; }
    public decimal? BackflowCommercialTestFee { get; set; }
    public decimal? CsiCommercialInspectionFee { get; set; }
    public decimal? CsiResidentialInspectionFee { get; set; }
    public decimal? FogTransportFee { get; set; }
    public decimal? FogInspectorFee { get; set; }
}
