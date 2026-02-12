
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
    public decimal? ResidentialFee { get; set; }

    [Precision(19, 4)]
    public decimal? CommercialFee { get; set; }

    [Precision(19, 4)]
    public decimal? FogFee { get; set; }
}