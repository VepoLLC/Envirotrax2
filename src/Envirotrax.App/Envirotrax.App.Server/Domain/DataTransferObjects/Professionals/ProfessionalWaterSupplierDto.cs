
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

    public decimal? BackflowResidentialTestFee { get; set; }
    public decimal? BackflowCommercialTestFee { get; set; }
    public decimal CsiCommercialInspectionFee { get; set; }
    public decimal CsiResidentialInspectionFee { get; set; }
    public decimal? FogTransportFee { get; set; }
}

public class AvailableWaterSupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int? StateId { get; set; }

    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInpection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }

    public bool BpatsRequireInsurance { get; set; }
    public bool CsiInspectorsRequireInsurance { get; set; }
    public bool FogTransportersRequireInsurance { get; set; }
    public bool FogVehiclesRequirePermit { get; set; }
    public bool FogVehiclesRequireInspection { get; set; }

    public decimal? BpatsRequireInsuranceAmount { get; set; }
    public decimal? CsiInspectorsRequireInsuranceAmount { get; set; }
    public decimal? FogTransportersRequireInsuranceAmount { get; set; }

    public decimal? BackflowResidentialTestFee { get; set; }
    public decimal? BackflowCommercialTestFee { get; set; }
    public decimal CsiCommercialInspectionFee { get; set; }
    public decimal CsiResidentialInspectionFee { get; set; }
    public decimal? FogTransportFee { get; set; }
}