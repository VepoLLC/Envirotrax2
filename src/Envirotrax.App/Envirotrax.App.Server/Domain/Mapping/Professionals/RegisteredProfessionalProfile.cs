
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Mapping.Professionals;

public class RegisteredProfessionalProfile : Profile
{
    public RegisteredProfessionalProfile()
    {
        // The projection and the DTO share every property name, so the map exists purely so
        // ConvertFilterProperties / ConvertSortProperties can translate the client's query.
        CreateMap<RegisteredProfessional, RegisteredProfessionalDto>();

        CreateMap<RegisteredProfessionalSupplier, RegisteredProfessionalSupplierDto>();
    }
}
