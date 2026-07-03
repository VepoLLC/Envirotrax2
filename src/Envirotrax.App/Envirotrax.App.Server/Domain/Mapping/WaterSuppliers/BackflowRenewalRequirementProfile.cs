using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.WaterSuppliers;

public class BackflowRenewalRequirementProfile : Profile
{
    public BackflowRenewalRequirementProfile()
    {
        CreateMap<BackflowRenewalRequirement, BackflowRenewalRequirementDto>().ReverseMap();
    }
}
