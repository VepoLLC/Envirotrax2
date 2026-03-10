using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Mapping.Csi;

public class CsiInspectionProfile : Profile
{
    public CsiInspectionProfile()
    {
        CreateMap<CsiInspection, CsiInspectionDto>()
            .ReverseMap();
    }
}
