using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Mapping.Sites;

public class SiteLogProfile : Profile
{
    public SiteLogProfile()
    {
        CreateMap<SiteLog, SiteLogDto>()
            .ForMember(dto => dto.Site, opt => opt.Ignore())
            .ForMember(dto => dto.Assembly, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                dto.Site ??= new ReferencedSiteDto { Id = model.SiteId };
                dto.Assembly ??= model.AssemblyId.HasValue
                    ? new ReferencedBackflowTestDto { Id = model.AssemblyId.Value }
                    : null;
                if (model.Assembly != null)
                {
                    dto.Assembly = new ReferencedBackflowTestDto
                    {
                        Id = model.Assembly.Id,
                        SerialNumber = model.Assembly.SerialNumber,
                        Manufacturer = model.Assembly.Manufacturer,
                        Model = model.Assembly.Model,
                        Size = model.Assembly.Size,
                        DeviceType = model.Assembly.DeviceType
                    };
                }
            })
            .ReverseMap()
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.SiteId, opt => opt.MapFrom(dto => dto.Site.Id ?? 0))
            .ForMember(m => m.Assembly, opt => opt.Ignore())
            .ForMember(m => m.AssemblyId, opt => opt.MapFrom(dto => dto.Assembly != null ? dto.Assembly.Id : (int?)null))
            .ForMember(m => m.CreatedBy, opt => opt.Ignore());

        CreateMap<BackflowTest, ReferencedBackflowTestDto>();

        CreateMap<SiteLog, PropertyLogDto>()
            .ForMember(dto => dto.Assembly, opt => opt.Ignore())
            .ForMember(dto => dto.Url, opt => opt.Ignore())
            .ForMember(dto => dto.ReviewDateStatus, opt => opt.Ignore())
            .AfterMap((model, dto) =>
            {
                if (model.Assembly != null)
                {
                    dto.Assembly = new ReferencedBackflowTestDto
                    {
                        Id = model.Assembly.Id,
                        SerialNumber = model.Assembly.SerialNumber,
                        Manufacturer = model.Assembly.Manufacturer,
                        Model = model.Assembly.Model,
                        Size = model.Assembly.Size,
                        DeviceType = model.Assembly.DeviceType
                    };
                }
                else if (model.AssemblyId.HasValue)
                {
                    dto.Assembly = new ReferencedBackflowTestDto { Id = model.AssemblyId.Value };
                }
            })
            .ReverseMap()
            .ForMember(m => m.Site, opt => opt.Ignore())
            .ForMember(m => m.Assembly, opt => opt.Ignore())
            .ForMember(m => m.CreatedBy, opt => opt.Ignore());
    }
}
