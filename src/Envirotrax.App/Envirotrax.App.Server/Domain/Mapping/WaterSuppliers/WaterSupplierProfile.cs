
using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Mapping.WaterSuppliers;

public class WaterSupplierProfile : Profile
{
    public WaterSupplierProfile()
    {
        CreateMap<WaterSupplier, WaterSupplierDto>()
            .AfterMap((model, dto) =>
            {
                if (model.ParentId.HasValue)
                {
                    dto.Parent ??= new ReferencedWaterSupplierDto
                    {
                        Id = model.ParentId.Value
                    };
                }
                dto.State ??= model.StateId.HasValue ? new ReferencedStateDto { Id = model.StateId } : null;
                dto.LetterState ??= model.LetterStateId.HasValue ? new ReferencedStateDto { Id = model.LetterStateId } : null;
                dto.LetterContactState ??= model.LetterContactStateId.HasValue ? new ReferencedStateDto { Id = model.LetterContactStateId } : null;
            })
            .ReverseMap()
            .ForMember(supplier => supplier.Parent, opt => opt.Ignore())
            .ForMember(supplier => supplier.ParentId, opt => opt.MapFrom(supplier => supplier.Parent!.Id))
            .ForMember(supplier => supplier.State, opt => opt.Ignore())
            .ForMember(supplier => supplier.StateId, opt => opt.MapFrom(dto => dto.State != null ? dto.State.Id : (int?)null))
            .ForMember(supplier => supplier.LetterState, opt => opt.Ignore())
            .ForMember(supplier => supplier.LetterStateId, opt => opt.MapFrom(dto => dto.LetterState != null ? dto.LetterState.Id : (int?)null))
            .ForMember(supplier => supplier.LetterContactState, opt => opt.Ignore())
            .ForMember(supplier => supplier.LetterContactStateId, opt => opt.MapFrom(dto => dto.LetterContactState != null ? dto.LetterContactState.Id : (int?)null));

        CreateMap<WaterSupplier, ReferencedWaterSupplierDto>()
            .AfterMap((ws, dto) =>
            {
                dto.State ??= ws.StateId.HasValue ? new ReferencedStateDto { Id = ws.StateId } : null;
            })
            .ReverseMap()
            .ForMember(ws => ws.State, opt => opt.Ignore())
            .ForMember(ws => ws.StateId, opt => opt.MapFrom(dto => dto.State != null ? dto.State.Id : (int?)null));
    }
}