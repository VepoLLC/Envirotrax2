using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Mapping.Csi;

public class CsiInspectionImageProfile : Profile
{
    public CsiInspectionImageProfile()
    {
        CreateMap<CsiInspectionImage, CsiInspectionImageDto>()
            .ForMember(dto => dto.Url, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(m => m.Inspection, opt => opt.Ignore())
            .ForMember(m => m.WaterSupplier, opt => opt.Ignore())
            .ForMember(m => m.Professional, opt => opt.Ignore())
            .ForMember(m => m.FilePath, opt => opt.Ignore());
    }
}
