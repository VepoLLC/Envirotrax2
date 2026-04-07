using AutoMapper;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.Sites;

public class SiteProfile : Profile
{
    public SiteProfile()
    {
        CreateMap<Site, SiteDto>()
            .AfterMap((model, dto) =>
            {
                dto.WaterSupplier ??= new ReferencedWaterSupplierDto
                {
                    Id = model.WaterSupplierId
                };
                dto.StateCode ??= model.State?.Code;
                dto.MailingStateCode ??= model.MailingState?.Code;
            })
            .ReverseMap()
            .ForMember(s => s.WaterSupplier, opt => opt.Ignore())
            .ForMember(s => s.WaterSupplierId, opt => opt.MapFrom(dto => dto.WaterSupplier!.Id));
    }
}
