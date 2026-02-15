
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalWaterSupplier : TenantModel<WaterSupplier>, IProfessionalModel
{
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInpection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }

    public bool IsBanned { get; set; }

    [Precision(19, 4)]
    public decimal? BackflowResidentialTestFee { get; set; }

    [Precision(19, 4)]
    public decimal? BackflowCommercialTestFee { get; set; }

    [Precision(19, 4)]
    public decimal? CsiCommercialInspectionFee { get; set; }

    [Precision(19, 4)]
    public decimal? CsiResidentialInspectionFee { get; set; }

    [Precision(19, 4)]
    public decimal? FogTransportFee { get; set; }
}

public class AvailableWaterSupplier
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

    [Precision(19, 4)]
    public decimal? BackflowCommercialTestFee { get; set; }

    [Precision(19, 4)]
    public decimal? CsiCommercialInspectionFee { get; set; }

    [Precision(19, 4)]
    public decimal? CsiResidentialInspectionFee { get; set; }

    [Precision(19, 4)]
    public decimal? FogTransportFee { get; set; }
}