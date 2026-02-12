
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

public class ProfessionalWaterSupplierDto
{
    [Required]
    public ReferencedWaterSupplierDto WaterSupplier { get; set; } = null!;

    public ReferencedProfessionalDto? Professional { get; set; }

    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInpection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }

    public bool IsBanned { get; set; }

    public decimal? ResidentialFee { get; set; }

    public decimal? CommercialFee { get; set; }

    public decimal? FogFee { get; set; }
}